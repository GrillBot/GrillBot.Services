﻿using Discord;
using GrillBot.Core.Extensions;
using GrillBot.Core.Extensions.Discord;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
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
    IServiceProvider serviceProvider,
    DiscordManager _discordManager,
    IServer _redisServer,
    IDistributedCache _cache
) : BaseEventHandlerWithDb<UserJoinedPayload, InviteContext>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        UserJoinedPayload message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        var guild = await _discordManager.GetGuildAsync(message.GuildId.ToUlong(), cancellationToken: cancellationToken);
        if (guild is null || !await guild.CanManageInvitesAsync(_discordManager.CurrentUser, cancellationToken))
            return RabbitConsumptionResult.Success;

        var user = await _discordManager.GetGuildUserAsync(guild.Id, message.UserId.ToUlong(), cancellationToken);
        if (user?.IsUser() != true)
            return RabbitConsumptionResult.Success;

        var latestInvites = await _discordManager.GetInvitesAsync(message.GuildId.ToUlong(), cancellationToken);

        var usedInvite = await FindUserInviteAsync(user, latestInvites, cancellationToken);

        if (usedInvite is null)
        {
            await ProcessUnknownInviteAsync(user, cancellationToken);
        }
        else
        {
            await ProcessInviteAsync(user, usedInvite, cancellationToken);
            await SyncCacheAsync(guild, cancellationToken);
        }

        return RabbitConsumptionResult.Success;
    }

    private async Task<InviteMetadata?> FindUserInviteAsync(IGuildUser user, List<IInviteMetadata> metadata, CancellationToken cancellationToken = default)
    {
        var possibleInvites = metadata
            .Select(InviteMetadata.Create)
            .Where(o => o.CreatedAt.GetValueOrDefault().ToUniversalTime() <= user.JoinedAt.GetValueOrDefault().UtcDateTime)
            .ToList();

        var cachedInvites = await GetCachedInvitesAsync(user.Guild, cancellationToken);

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

    private Task ProcessUnknownInviteAsync(IGuildUser user, CancellationToken cancellationToken = default)
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

        return Publisher.PublishAsync(new CreateItemsMessage(logRequest), cancellationToken: cancellationToken);
    }

    private async Task ProcessInviteAsync(IGuildUser user, InviteMetadata invite, CancellationToken cancellationToken = default)
    {
        var guildId = user.GuildId.ToString();

        var inviteEntityQuery = DbContext.Invites
            .Include(o => o.Uses)
            .Where(o => o.GuildId == guildId && o.Code == invite.Code);

        var inviteEntity = await ContextHelper.ReadFirstOrDefaultEntityAsync(inviteEntityQuery, cancellationToken);
        if (inviteEntity is null)
        {
            inviteEntity = new Invite
            {
                Code = invite.Code,
                CreatedAt = invite.CreatedAt,
                CreatorId = invite.CreatorId,
                GuildId = guildId
            };

            await DbContext.AddAsync(inviteEntity, cancellationToken);
        }

        inviteEntity.Uses.Add(new InviteUse
        {
            Code = invite.Code,
            GuildId = guildId,
            UsedAt = (user.JoinedAt ?? DateTimeOffset.UtcNow).UtcDateTime,
            UserId = user.Id.ToString()
        });

        await ContextHelper.SaveChangesAsync(cancellationToken);
    }

    private async Task<List<InviteMetadata>> GetCachedInvitesAsync(IGuild guild, CancellationToken cancellationToken = default)
    {
        var result = new List<InviteMetadata>();

        await foreach (var key in _redisServer.KeysAsync(pattern: $"InviteMetadata-{guild.Id}-*", pageSize: int.MaxValue))
        {
            var item = await _cache.GetAsync<InviteMetadata>(key.ToString(), cancellationToken);
            if (item is not null)
                result.Add(item);
        }

        return result;
    }

    private Task SyncCacheAsync(IGuild guild, CancellationToken cancellationToken = default)
    {
        var message = new SynchronizeGuildInvitesPayload(guild.Id.ToString(), true);
        return Publisher.PublishAsync(message, cancellationToken: cancellationToken);
    }
}
