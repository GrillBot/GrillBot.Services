using Discord;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace InviteService.Handlers;

public class UserJoinedEventHandler(
    ILoggerFactory loggerFactory,
    InviteContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher rabbitPublisher,
    DiscordManager _discordManager,
    IServer _redisServer,
    IDistributedCache _cache
) : BaseEventHandlerWithDb<UserJoinedPayload, InviteContext>(loggerFactory, dbContext, counterManager, rabbitPublisher)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(UserJoinedPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        var guild = await _discordManager.GetGuildAsync(message.GuildId.ToUlong());
        if (guild is null || !await guild.CanManageInvitesAsync(_discordManager.CurrentUser))
            return RabbitConsumptionResult.Success;

        var user = await guild.GetUserAsync(message.UserId.ToUlong());
        if (user?.IsUser() != true)
            return RabbitConsumptionResult.Success;

        var latestInvites = await _discordManager.GetInvitesAsync(message.GuildId.ToUlong());

        var usedInvite = await FindUserInviteAsync(user, latestInvites);

        if (usedInvite is null)
        {
            await ProcessUnknownInviteAsync(user);
        }
        else
        {
            await ProcessInviteAsync(user, usedInvite);
            await SyncCacheAsync(guild);
        }

        return RabbitConsumptionResult.Success;
    }

    private async Task<InviteMetadata?> FindUserInviteAsync(IGuildUser user, List<IInviteMetadata> metadata)
    {
        var possibleInvites = metadata
            .Select(InviteMetadata.Create)
            .Where(o => o.CreatedAt.GetValueOrDefault().ToUniversalTime() <= user.JoinedAt.GetValueOrDefault().UtcDateTime)
            .ToList();

        var cachedInvites = await GetCachedInvitesAsync(user.Guild);

        // Try find invite with limited count of usage. If invite was used as last, discord will automatically remove this invite.
        var missingInvite = cachedInvites
            .OrderByDescending(o => o.CreatedAt ?? DateTimeOffset.MinValue)
            .FirstOrDefault(o => !possibleInvites.Select(x => x.Code).Contains(o.Code));

        if (missingInvite is not null)
            return missingInvite;

        // Find invite which have incremented use count against the cache.
        var result = cachedInvites
            .Select(o => new
            {
                Cached = o,
                Current = possibleInvites.Find(x => x.Code == o.Code)
            })
            .FirstOrDefault(o => o.Current is not null && o.Current.Uses > o.Cached.Uses);

        return result?.Current;
    }

    private Task ProcessUnknownInviteAsync(IGuildUser user)
    {
        var guildId = user.GuildId.ToString();
        var userId = user.Id.ToString();
        var joinedAt = (user.JoinedAt ?? DateTimeOffset.UtcNow).UtcDateTime;

        var logRequest = new LogRequest(LogType.Warning, joinedAt, guildId, userId, null, null)
        {
            LogMessage = new LogMessageRequest
            {
                Message = $"User \"{user.GetFullName()}\" ({userId}) used unknown invite.",
                Source = $"{CounterKey} {nameof(UserJoinedEventHandler)}",
                SourceAppName = "InviteService"
            }
        };

        return Publisher.PublishAsync(new CreateItemsMessage(logRequest));
    }

    private async Task ProcessInviteAsync(IGuildUser user, InviteMetadata invite)
    {
        var guildId = user.GuildId.ToString();

        var inviteEntityQuery = DbContext.Invites
            .Include(o => o.Uses)
            .Where(o => o.GuildId == guildId && o.Code == invite.Code);

        var inviteEntity = await ContextHelper.ReadFirstOrDefaultEntityAsync(inviteEntityQuery);
        if (inviteEntity is null)
        {
            inviteEntity = new Invite
            {
                Code = invite.Code,
                CreatedAt = invite.CreatedAt,
                CreatorId = invite.CreatorId,
                GuildId = guildId
            };

            await DbContext.AddAsync(inviteEntity);
        }

        inviteEntity.Uses.Add(new InviteUse
        {
            Code = invite.Code,
            GuildId = guildId,
            UsedAt = (user.JoinedAt ?? DateTimeOffset.UtcNow).UtcDateTime,
            UserId = user.Id.ToString()
        });

        await ContextHelper.SaveChagesAsync();
    }

    private async Task<List<InviteMetadata>> GetCachedInvitesAsync(IGuild guild)
    {
        var cursor = 0L;
        var result = new List<InviteMetadata>();

        do
        {
            var keys = _redisServer.KeysAsync(pattern: $"InviteMetadata-{guild.Id}-*", pageSize: 100, cursor: cursor);
            await foreach (var key in keys)
            {
                var item = await _cache.GetAsync<InviteMetadata>(key.ToString());
                if (item is not null)
                    result.Add(item);
            }

            cursor = ((IScanningCursor)keys).Cursor;
        }
        while (cursor != 0);

        return result;
    }

    private Task SyncCacheAsync(IGuild guild)
    {
        var message = new SynchronizeGuildInvitesPayload(guild.Id.ToString(), true);
        return Publisher.PublishAsync(message);
    }
}
