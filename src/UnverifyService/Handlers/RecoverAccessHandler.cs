using Discord;
using GrillBot.Core.Extensions;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.Services.AuditLog.Enums;
using GrillBot.Core.Services.AuditLog.Models.Events.Create;
using GrillBot.Services.Common.Discord;
using Microsoft.EntityFrameworkCore;
using UnverifyService.Core.Entity.Logs;
using UnverifyService.Core.Enums;
using UnverifyService.Models.Events;

namespace UnverifyService.Handlers;

public class RecoverAccessHandler(
    IServiceProvider serviceProvider,
    DiscordManager _discordManager
) : UnverifyServiceBaseHandler<RecoverAccessMessage>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> ProcessHandlerAsync(
        RecoverAccessMessage message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        var logItem = await FindLogItemAsync(message, cancellationToken);
        if (logItem is null)
        {
            var logRequest = new LogRequest(LogType.Warning, DateTime.UtcNow, userId: currentUser.Id)
            {
                LogMessage = new LogMessageRequest(
                    $"Unable to find log item for recovery process. LogId: {message.LogId?.ToString() ?? "<null>"}. LogNumber: {message.LogNumber?.ToString() ?? "<null>"}",
                    "UnverifyService",
                    GetType().Name
                )
            };

            await Publisher.PublishAsync(new CreateItemsMessage(logRequest), cancellationToken: cancellationToken);
            return RabbitConsumptionResult.Reject;
        }

        if (await IsAnyActiveUnverifyAsync(logItem, cancellationToken))
        {
            var logRequest = new LogRequest(LogType.Warning, DateTime.UtcNow, logItem.GuildId.ToString(), currentUser.Id)
            {
                LogMessage = new LogMessageRequest(
                    $"Unable to recover access because target user ({logItem.ToUserId}) have active unverify.",
                    "UnverifyService",
                    GetType().Name
                )
            };

            await Publisher.PublishAsync(new CreateItemsMessage(logRequest), cancellationToken: cancellationToken);
            return RabbitConsumptionResult.Reject;
        }

        var targetUser = await _discordManager.GetGuildUserAsync(logItem.GuildId, logItem.ToUserId, cancellationToken);
        if (targetUser is null)
        {
            // There is nothing to return when the user is no longer on the server.
            // Only send metrics recalculation to refresh prometheus data.
            await RecalculateMetricsAsync(cancellationToken);
            return RabbitConsumptionResult.Success;
        }

        var discordGuild = await _discordManager.GetGuildAsync(logItem.GuildId, false, cancellationToken);
        if (discordGuild is null)
        {
            // There is nothing to return when bot is no longer on the server.
            // Only send metrics recalculation to refresh prometheus data.
            await RecalculateMetricsAsync(cancellationToken);
            return RabbitConsumptionResult.Success;
        }

        var muteRole = await FindMutedRoleAsync(discordGuild, logItem, cancellationToken);

        var rolesToReturn = logItem.SetOperation!.Roles
            .Where(o => !o.IsKept && targetUser.RoleIds.All(x => x != o.RoleId))
            .Select(o => discordGuild.GetRole(o.RoleId))
            .Where(role => role is not null)
            .ToList();

        var channelsToReturn = new List<(IGuildChannel channel, OverwritePermissions perms)>();
        foreach (var channel in logItem.SetOperation.Channels.Where(o => !o.IsKept))
        {
            var guildChannel = await discordGuild.GetChannelAsync(channel.ChannelId, options: new() { CancelToken = cancellationToken });
            var perms = guildChannel?.GetPermissionOverwrite(targetUser);

            if (perms is null || (perms.Value.AllowValue == channel.AllowValue && perms.Value.DenyValue == channel.DenyValue))
                continue;

            channelsToReturn.Add((guildChannel!, perms.Value));
        }

        await LogRecoveryAsync(logItem, rolesToReturn, channelsToReturn, currentUser, cancellationToken);

        if (rolesToReturn.Count > 0)
            await targetUser.AddRolesAsync(rolesToReturn, new() { CancelToken = cancellationToken });

        foreach (var (channel, perms) in channelsToReturn)
            await channel.AddPermissionOverwriteAsync(targetUser, perms, new() { CancelToken = cancellationToken });

        if (muteRole is not null && !logItem.SetOperation!.KeepMutedRole)
            await targetUser.RemoveRoleAsync(muteRole, new() { CancelToken = cancellationToken });

        await RecalculateMetricsAsync(cancellationToken);
        return RabbitConsumptionResult.Success;
    }

    private async Task<UnverifyLogItem?> FindLogItemAsync(RecoverAccessMessage message, CancellationToken cancellationToken = default)
    {
        var query = DbContext.LogItems
            .Include(o => o.SetOperation).ThenInclude(o => o!.Channels)
            .Include(o => o.SetOperation).ThenInclude(o => o!.Roles)
            .Where(o => (o.OperationType == UnverifyOperationType.Unverify || o.OperationType == UnverifyOperationType.SelfUnverify) && o.SetOperation != null);

        if (message.LogNumber is not null)
            query = query.Where(o => o.LogNumber == message.LogNumber.Value);
        else if (message.LogId is not null)
            query = query.Where(o => o.Id == message.LogId.Value);
        else
            return null;

        return await ContextHelper.ReadFirstOrDefaultEntityAsync(query, cancellationToken);
    }

    private Task<bool> IsAnyActiveUnverifyAsync(UnverifyLogItem logItem, CancellationToken cancellationToken = default)
    {
        var query = ContextHelper.DbContext.ActiveUnverifies.AsNoTracking()
            .Where(o => o.LogItem.GuildId == logItem.GuildId && o.LogItem.ToUserId == logItem.ToUserId);

        return ContextHelper.IsAnyAsync(query, cancellationToken);
    }

    private async Task<IRole?> FindMutedRoleAsync(IGuild? discordGuild, UnverifyLogItem logItem, CancellationToken cancellationToken = default)
    {
        if (discordGuild is null)
            return null;

        var guildQuery = DbContext.Guilds.AsNoTracking().Where(o => o.GuildId == logItem.GuildId);
        var guild = await ContextHelper.ReadFirstOrDefaultEntityAsync(guildQuery, cancellationToken);
        if (guild?.MuteRoleId is null)
            return null;

        return discordGuild.GetRole(guild.MuteRoleId.Value) ??
            await discordGuild.GetRoleAsync(guild.MuteRoleId.Value, new() { CancelToken = cancellationToken });
    }

    private async Task LogRecoveryAsync(
        UnverifyLogItem parentLogItem,
        List<IRole> returnedRoles,
        List<(IGuildChannel channel, OverwritePermissions perms)> returnedChannels,
        ICurrentUserProvider currentUser,
        CancellationToken cancellationToken = default
    )
    {
        var logItem = new UnverifyLogItem
        {
            CreatedAt = DateTime.UtcNow,
            FromUserId = currentUser.Id.ToUlong(),
            GuildId = parentLogItem.GuildId,
            Id = Guid.NewGuid(),
            OperationType = UnverifyOperationType.Recovery,
            ParentLogItem = parentLogItem,
            ParentLogItemId = parentLogItem.Id,
            ToUserId = parentLogItem.ToUserId,
            RemoveOperation = new UnverifyLogRemoveOperation
            {
                Channels = [.. returnedChannels.Select(o => new UnverifyLogRemoveChannel
                {
                    AllowValue = o.perms.AllowValue,
                    ChannelId = o.channel.Id,
                    DenyValue = o.perms.DenyValue
                })],
                Force = false,
                IsFromWeb = currentUser.Role != "Command",
                Language = null,
                Roles = [.. returnedRoles.Select(o => new UnverifyLogRemoveRole
                {
                    RoleId = o.Id
                })]
            }
        };

        await ContextHelper.DbContext.AddAsync(logItem, cancellationToken);
        await ContextHelper.SaveChangesAsync(cancellationToken);
    }

    private Task RecalculateMetricsAsync(CancellationToken cancellationToken = default)
        => Publisher.PublishAsync(new RecalculateMetricsMessage(), cancellationToken: cancellationToken);
}
