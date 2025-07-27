using Discord;
using GrillBot.Core.Exceptions;
using GrillBot.Core.Extensions;
using GrillBot.Core.Extensions.Discord;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Core.Services.AuditLog.Enums;
using GrillBot.Core.Services.AuditLog.Models.Events.Create;
using GrillBot.Core.Services.GrillBot.Models;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages;
using GrillBot.Core.Services.UserMeasures.Models.Events;
using GrillBot.Services.Common.Discord;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using UnverifyService.Core.Entity;
using UnverifyService.Core.Entity.Logs;
using UnverifyService.Core.Enums;
using UnverifyService.Models;
using UnverifyService.Models.Events;
using UnverifyService.Models.Response;

namespace UnverifyService.Actions;

public class RemoveUnverifyAction(
    IServiceProvider serviceProvider,
    DiscordManager _discordManager,
    IRabbitPublisher _rabbitPublisher
) : ApiAction<UnverifyContext>(serviceProvider)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        if (!CurrentUser.IsLogged)
            return new ApiResult(StatusCodes.Status403Forbidden, new { Message = "User is not logged. Missing Authorization token" });

        var guildId = GetParameter<ulong>(0);
        var userId = GetParameter<ulong>(1);
        var isForceRemove = GetOptionalParameter<bool>(2);
        var isAutoRemove = CurrentUser.Id == _discordManager.CurrentUser.Id.ToString();

        IGuildUser? targetUser = null;
        try
        {
            targetUser = await _discordManager.GetGuildUserAsync(guildId, userId, CancellationToken);
            if (targetUser is null)
                return ApiResult.NotFound(new RemoveUnverifyResponse("Unverify/DestUserNotFound", 0, 0));

            var unverify = await FindUnverifyAsync(guildId, userId);
            if (unverify is null)
                return ApiResult.NotFound(new RemoveUnverifyResponse(new("Unverify/Message/RemoveAccessUnverifyNotFound", [targetUser.GetDisplayName()]), 0, 0));

            var session = await CreateUnverifySessionAsync(targetUser, unverify);
            await WriteToLogAsync(session, unverify, isAutoRemove, isForceRemove);

            if (!isForceRemove)
            {
                if (!isAutoRemove)
                    await NotifyUserMeasuresAsync(unverify);

                if (
                    session.MutedRole is not null &&
                    session.TargetUser.RoleIds.Contains(session.MutedRole.Id) &&
                    !session.KeepMutedRole
                )
                {
                    await session.TargetUser.RemoveRoleAsync(session.MutedRole);
                }

                foreach (var channel in session.ChannelsToRemove)
                    await channel.Item1.AddPermissionOverwriteAsync(session.TargetUser, channel.Item2, new() { CancelToken = CancellationToken });
                await session.TargetUser.AddRolesAsync(session.RolesToRemove, new() { CancelToken = CancellationToken });
            }

            try
            {
                ContextHelper.DbContext.Remove(unverify);
                await ContextHelper.SaveChangesAsync(CancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var logRequest = new LogRequest(LogType.Warning, DateTime.UtcNow, unverify.LogItem.GuildId.ToString(), CurrentUser.Id)
                {
                    LogMessage = new LogMessageRequest(
                        $"A database error occured while processing unverify.\n{ex}",
                        "UnverifyService",
                        nameof(RemoveUnverifyAction)
                    )
                };

                await _rabbitPublisher.PublishAsync(new CreateItemsMessage(logRequest), cancellationToken: CancellationToken);
            }

            if (isAutoRemove)
                return CreateSuccessResponse(session);

            await SendNotificationToUserAsync(session);
            return CreateSuccessResponse(session);
        }
        catch (Exception ex) when (!isAutoRemove)
        {
            var response = new RemoveUnverifyResponse(
                new LocalizedMessageContent("Unverify/Message/ManuallyRemoveFailed", [targetUser?.GetDisplayName() ?? "", ex.Message]),
                0, 0
            );

            return new ApiResult(StatusCodes.Status500InternalServerError, response);
        }
        finally
        {
            await RecalculateMetricsAsync();
        }
    }

    private Task<ActiveUnverify?> FindUnverifyAsync(ulong guildId, ulong userId)
    {
        var query = ContextHelper.DbContext.ActiveUnverifies
            .Include(o => o.LogItem.SetOperation).ThenInclude(o => o!.Roles)
            .Include(o => o.LogItem.SetOperation).ThenInclude(o => o!.Channels)
            .Where(o => o.LogItem.GuildId == guildId && o.LogItem.ToUserId == userId)
            .AsSplitQuery();

        return ContextHelper.ReadFirstOrDefaultEntityAsync(query, CancellationToken);
    }

    private async Task<UnverifySession> CreateUnverifySessionAsync(IGuildUser targetUser, ActiveUnverify unverify)
    {
        var targetUserQuery = ContextHelper.DbContext.Users.AsNoTracking().Where(o => o.Id == targetUser.Id);
        var targetUserEntity = await ContextHelper.ReadFirstOrDefaultEntityAsync(targetUserQuery, CancellationToken);
        var isSelfUnverify = unverify.LogItem.OperationType == UnverifyOperationType.SelfUnverify;
        var guildEntityQuery = ContextHelper.DbContext.Guilds.AsNoTracking().Where(o => o.GuildId == targetUser.GuildId);
        var guildEntity = await ContextHelper.ReadFirstOrDefaultEntityAsync(guildEntityQuery, CancellationToken);

        var session = new UnverifySession(targetUser, targetUserEntity, unverify.StartAtUtc, unverify.EndAtUtc, unverify.LogItem.SetOperation!.Reason, isSelfUnverify)
        {
            MutedRole = guildEntity?.MuteRoleId is null ? null : targetUser.Guild.GetRole(guildEntity.MuteRoleId.Value)
        };

        foreach (var logRole in unverify.LogItem.SetOperation.Roles)
        {
            var role = targetUser.Guild.GetRole(logRole.RoleId);

            if (logRole.IsKept)
                session.RolesToKeep.Add(role);
            else
                session.RolesToRemove.Add(role);
        }

        foreach (var logChannel in unverify.LogItem.SetOperation.Channels)
        {
            var guildChannel = await targetUser.Guild.GetChannelAsync(logChannel.ChannelId, options: new() { CancelToken = CancellationToken });
            if (guildChannel is null)
                continue;

            if (logChannel.IsKept)
                session.ChannelsToKeep.Add((guildChannel, logChannel.Perms));
            else
                session.ChannelsToRemove.Add((guildChannel, logChannel.Perms));
        }

        return session;
    }

    private async Task WriteToLogAsync(UnverifySession session, ActiveUnverify unverify, bool isAutoRemove, bool isForceRemove)
    {
        var currentUserId = CurrentUser.Id.ToUlong();
        var botUserQuery = ContextHelper.DbContext.Users.Where(o => o.Id == currentUserId);
        var botUserEntity = await ContextHelper.ReadFirstOrDefaultEntityAsync(botUserQuery, CancellationToken);

        var logItem = new UnverifyLogItem
        {
            CreatedAt = DateTime.UtcNow,
            FromUserId = currentUserId,
            GuildId = unverify.LogItem.GuildId,
            Id = Guid.NewGuid(),
            OperationType = isAutoRemove ? UnverifyOperationType.AutoRemove : UnverifyOperationType.ManualRemove,
            ParentLogItem = unverify.LogItem,
            ParentLogItemId = unverify.LogSetId,
            ToUserId = session.TargetUser.Id,
            RemoveOperation = new UnverifyLogRemoveOperation
            {
                Force = isForceRemove,
                IsFromWeb = CurrentUser.Role != "Command" && !isAutoRemove,
                Language = unverify.LogItem.SetOperation?.Language ?? botUserEntity?.Language ?? "cs",
                Roles = isForceRemove ?
                    [] :
                    [.. session.RolesToRemove.Select(o => new UnverifyLogRemoveRole { RoleId = o.Id })],
                Channels = isForceRemove ?
                    [] :
                    [.. session.ChannelsToRemove.Select(o => new UnverifyLogRemoveChannel{
                        AllowValue = o.Item2.AllowValue,
                        ChannelId = o.Item1.Id,
                        DenyValue = o.Item2.DenyValue
                    })]
            }
        };

        await ContextHelper.DbContext.AddAsync(logItem, CancellationToken);
        await ContextHelper.SaveChangesAsync(CancellationToken);
    }

    private Task NotifyUserMeasuresAsync(ActiveUnverify unverify)
    {
        var payload = new UnverifyModifyPayload(unverify.LogItem.LogNumber, DateTime.UtcNow);
        return _rabbitPublisher.PublishAsync(payload, cancellationToken: CancellationToken);
    }

    private static ApiResult CreateSuccessResponse(UnverifySession session)
    {
        return ApiResult.Ok(
            new RemoveUnverifyResponse(
                new LocalizedMessageContent("Unverify/Message/ManuallyRemoveToChannel", [session.TargetUser.GetDisplayName()]),
                session.RolesToRemove.Count,
                session.ChannelsToRemove.Count
            )
        );
    }

    private Task SendNotificationToUserAsync(UnverifySession session)
    {
        var message = new DiscordSendMessagePayload(
            guildId: null,
            channelId: session.TargetUser.Id,
            new LocalizedMessageContent(
                "Unverify/Message/PrivateManuallyRemovedUnverify",
                [session.TargetUser.Guild.Name]
            ),
            attachments: [],
            "Unverify"
        );

        message.WithLocalization(locale: session.TargetUserEntity?.Language ?? "cs");
        return _rabbitPublisher.PublishAsync(message, cancellationToken: CancellationToken);
    }

    private Task RecalculateMetricsAsync()
        => _rabbitPublisher.PublishAsync(new RecalculateMetricsMessage(), cancellationToken: CancellationToken);
}
