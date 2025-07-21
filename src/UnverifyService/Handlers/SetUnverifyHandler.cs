using Discord;
using GrillBot.Core.Extensions.Discord;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.Services.AuditLog.Enums;
using GrillBot.Core.Services.AuditLog.Models.Events.Create;
using GrillBot.Services.Common.Discord;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using UnverifyService.Actions;
using UnverifyService.Core.Entity;
using UnverifyService.Models;
using UnverifyService.Models.Events;

namespace UnverifyService.Handlers;

public partial class SetUnverifyHandler(
    IServiceProvider serviceProvider,
    CheckUnverifyRequirementsAction _unverifyCheck,
    DiscordManager _discordManager
) : BaseEventHandlerWithDb<SetUnverifyMessage, UnverifyContext>(serviceProvider)
{
    private static readonly JsonSerializerOptions _serializerOptions = new() { WriteIndented = false };

    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        SetUnverifyMessage message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        if (!currentUser.IsLogged)
        {
            await NotifyUnauthorizedExecution(message, cancellationToken);
            return RabbitConsumptionResult.Reject;
        }

        var isValidUnverify = await CheckUnverifyRequirementsAsync(message, currentUser, cancellationToken);
        if (!isValidUnverify)
            return RabbitConsumptionResult.Reject;

        var session = await CreateSessionAsync(message, cancellationToken);
        if (message.TestRun)
        {
            await SendUnverifyMessageToChannelAsync(session, message, currentUser, cancellationToken);
            return RabbitConsumptionResult.Success;
        }

        var dbStrategy = DbContext.Database.CreateExecutionStrategy();
        return await dbStrategy.ExecuteAsync(async cancelToken =>
        {
            await using var transaction = await DbContext.Database.BeginTransactionAsync(cancelToken);

            var logItem = await LogUnverifyAsync(session, currentUser, cancelToken);
            await NotifyUserMeasuresAsync(session, logItem, currentUser, cancelToken);

            try
            {
                await ProcessUnverifySetAsync(session, logItem, cancelToken);
                await SendSuccessSetDMAsync(session, cancelToken);
                await SendUnverifyMessageToChannelAsync(session, message, currentUser, cancelToken);

                await transaction.CommitAsync(cancelToken);
                return RabbitConsumptionResult.Success;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occured while removing access to {FullName}", session.TargetUser.GetFullName());
                await RollbackAccessAsync(session, cancelToken);
                await transaction.RollbackAsync(cancelToken);

                return RabbitConsumptionResult.Retry;
            }
        }, cancellationToken);
    }

    private async Task<bool> CheckUnverifyRequirementsAsync(SetUnverifyMessage message, ICurrentUserProvider currentUser, CancellationToken cancellationToken = default)
    {
        _unverifyCheck.Init(null!, [message], currentUser);
        _unverifyCheck.SetCancellationToken(cancellationToken);

        var unverifyCheckResult = await _unverifyCheck.ProcessAsync();
        if (unverifyCheckResult.StatusCode == StatusCodes.Status200OK)
            return true;

        var logData = JsonSerializer.Serialize(unverifyCheckResult.Data, _serializerOptions);
        var logRequest = new LogRequest(LogType.Warning, DateTime.UtcNow, message.GuildId.ToString(), message.UserId.ToString())
        {
            LogMessage = new(
                $"Unverify request does not meet the requirements.\n{logData}",
                "UnverifyService",
                GetType().Name
            )
        };

        await Publisher.PublishAsync(new CreateItemsMessage(logRequest), cancellationToken: cancellationToken);
        return false;
    }

    private async Task<UnverifySession> CreateSessionAsync(SetUnverifyMessage message, CancellationToken cancellationToken = default)
    {
        var guild = (await _discordManager.GetGuildAsync(message.GuildId, false, cancellationToken))!;
        var targetUser = (await _discordManager.GetGuildUserAsync(message.GuildId, message.UserId, cancellationToken))!;
        var reason = message.IsSelfUnverify ? null : (message.Reason ?? "").Trim();
        var keepablesQuery = DbContext.SelfUnverifyKeepables.AsNoTracking().GroupBy(o => o.Group);
        var keepables = await ContextHelper.ReadToDictionaryAsync(keepablesQuery, o => o.Key, o => o.Select(x => x.Name).ToList(), cancellationToken);
        var guildQuery = DbContext.Guilds.AsNoTracking().Where(o => o.GuildId == message.GuildId);
        var guildEntity = await ContextHelper.ReadFirstOrDefaultEntityAsync(guildQuery, cancellationToken);
        var mutedRole = guildEntity?.MuteRoleId == null ? null : guild.GetRole(guildEntity.MuteRoleId.Value);
        var dbUserQuery = DbContext.Users.AsNoTracking().Where(o => o.Id == targetUser.Id);
        var dbUser = await ContextHelper.ReadFirstOrDefaultEntityAsync(dbUserQuery, cancellationToken);

        var session = new UnverifySession(targetUser, dbUser, DateTime.UtcNow, message.EndAtUtc, reason, message.IsSelfUnverify);

        await ProcessRolesAsync(session, keepables, mutedRole, message.RequiredKeepables, cancellationToken);
        await ProcessChannelsAsync(session, message.RequiredKeepables, cancellationToken);

        return session;
    }

    private async Task ProcessRolesAsync(
        UnverifySession session,
        Dictionary<string, List<string>> keepableDefinitions,
        IRole? mutedRole,
        List<string> requiredKeepables,
        CancellationToken cancellationToken = default
    )
    {
        session.RolesToRemove.AddRange(session.TargetUser.GetRoles());

        // Skip roles higher than bot for selfunverify purposes.
        if (session.IsSelfUnverify)
        {
            var botUser = (await _discordManager.GetGuildUserAsync(session.TargetUser.GuildId, _discordManager.CurrentUser.Id, cancellationToken))!;
            var botRolePosition = botUser.GetRoles().Max(o => o.Position);
            var rolesToKeep = session.RolesToRemove.FindAll(o => o.Position >= botRolePosition);

            if (rolesToKeep.Count > 0)
            {
                session.RolesToKeep.AddRange(rolesToKeep);
                session.RolesToRemove.RemoveAll(o => rolesToKeep.Exists(x => x.Id == o.Id));
            }
        }

        // Do not try remove muted role and roles managed by discord.
        var unavailableRoles = session.RolesToRemove.FindAll(o => o.IsManaged || (mutedRole is not null && o.Id == mutedRole.Id));
        if (unavailableRoles.Count > 0)
        {
            session.RolesToKeep.AddRange(unavailableRoles);
            session.RolesToRemove.RemoveAll(o => unavailableRoles.Exists(x => x.Id == o.Id));

            // Keep muting role while access returning.
            session.MutedRole = mutedRole;
        }

        foreach (var keepable in requiredKeepables)
        {
            var role = session.RolesToRemove.Find(o => string.Equals(o.Name, keepable, StringComparison.InvariantCultureIgnoreCase));

            if (role is not null)
            {
                session.RolesToKeep.Add(role);
                session.RolesToRemove.Remove(role);
                continue;
            }

            foreach (var groupKey in keepableDefinitions.Where(o => o.Value.Contains(keepable)).Select(o => o.Key))
            {
                role = session.RolesToRemove.Find(o => string.Equals(o.Name, groupKey == "_" ? keepable : groupKey, StringComparison.InvariantCultureIgnoreCase));
                if (role is null)
                    continue;

                session.RolesToKeep.Add(role);
                session.RolesToRemove.Remove(role);
            }
        }
    }

    private static async Task ProcessChannelsAsync(
        UnverifySession session,
        List<string> requiredKeepables,
        CancellationToken cancellationToken = default
    )
    {
        var channels = (await session.TargetUser.Guild.GetChannelsAsync(options: new() { CancelToken = cancellationToken })).ToList();
        channels = channels.FindAll(o => o is (IVoiceChannel or ITextChannel or IForumChannel) and not IThreadChannel); // Select channels, but ignore threads.

        var channelsToRemove = channels
            .Select(channel => (channel, channel.GetPermissionOverwrite(session.TargetUser) ?? OverwritePermissions.InheritAll))
            .Where(o => o.Item2.AllowValue > 0 || o.Item2.DenyValue > 0);

        session.ChannelsToRemove.AddRange(channelsToRemove);
        foreach (var keepable in requiredKeepables)
        {
            foreach (var overwrite in session.ChannelsToRemove.ToList())
            {
                if (!string.Equals(overwrite.Item1.Name, keepable, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                session.ChannelsToKeep.Add(overwrite);
                session.ChannelsToRemove.RemoveAll(o => o.Item1.Id == overwrite.Item1.Id);
            }
        }
    }
}
