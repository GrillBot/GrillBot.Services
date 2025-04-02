using GrillBot.Core.Extensions;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Core.Redis.Extensions;
using GrillBot.Core.Services.AuditLog.Enums;
using GrillBot.Core.Services.AuditLog.Models.Events.Create;
using GrillBot.Services.Common.Discord;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using InviteService.Core.Entity;
using InviteService.Extensions;
using InviteService.Models.Cache;
using InviteService.Models.Events;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace InviteService.Handlers;

public class SynchronizeGuildInvitesEventHandler(
    ILoggerFactory loggerFactory,
    InviteContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher publisher,
    IServer _redisServer,
    IDatabase _redisDatabase,
    DiscordManager _discordManager,
    IDistributedCache _cache
) : BaseEventHandlerWithDb<SynchronizeGuildInvitesPayload, InviteContext>(loggerFactory, dbContext, counterManager, publisher)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(SynchronizeGuildInvitesPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        await ClearInvitesForGuildAsync(message.GuildId);
        await DownloadInvitesToCacheAsync(message.GuildId, currentUser);

        return RabbitConsumptionResult.Success;
    }

    private async Task ClearInvitesForGuildAsync(string guildId)
    {
        var prefix = $"InviteMetadata-{guildId}-*";
        var cursor = 0L;

        do
        {
            var keysResult = _redisServer.KeysAsync(pattern: prefix, pageSize: 100, cursor: cursor);
            await foreach (var key in keysResult)
                await _redisDatabase.KeyDeleteAsync(key);

            cursor = ((IScanningCursor)keysResult).Cursor;
        }
        while (cursor != 0);
    }

    private async Task DownloadInvitesToCacheAsync(string guildId, ICurrentUserProvider currentUser)
    {
        var guild = await _discordManager.GetGuildAsync(guildId.ToUlong());

        if (guild is null || !await guild.CanManageInvitesAsync(_discordManager.CurrentUser))
            return;

        var invites = await _discordManager.GetInvitesAsync(guildId.ToUlong());
        if (invites.Count == 0)
            return;

        var logRequest = new LogRequest(LogType.Info, DateTime.UtcNow, guildId, currentUser.Id, null, null)
        {
            LogMessage = new LogMessageRequest
            {
                Message = $"Invites for guild \"{guild.Name}\" was loaded. Loaded invites: {invites.Count}",
                Source = $"{CounterKey} ({nameof(SynchronizeGuildInvitesEventHandler)})",
                SourceAppName = "InviteService"
            }
        };

        await Publisher.PublishAsync(new CreateItemsMessage(logRequest));

        foreach (var invite in invites)
        {
            var metadata = new InviteMetadata(invite.Code, invite.Uses ?? 0, invite.Inviter?.Id.ToString(), invite.CreatedAt?.UtcDateTime);
            var key = $"InviteMetadata-{invite.GuildId}-{invite.Code}";

            await _cache.SetAsync(key, metadata, null);
        }
    }
}
