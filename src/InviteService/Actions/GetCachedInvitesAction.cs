using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models.Pagination;
using GrillBot.Core.Redis.Extensions;
using GrillBot.Services.Common.Infrastructure.Api;
using InviteService.Models.Cache;
using InviteService.Models.Request;
using InviteService.Models.Response;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace InviteService.Actions;

public class GetCachedInvitesAction(
    ICounterManager counterManager,
    IServer _redisServer,
    IDistributedCache _cache
) : ApiAction(counterManager)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<InviteListRequest>(0);
        var invites = await ReadCachedInvitesAsync(request);

        var result = await PaginatedResponse<Invite>.CopyAndMapAsync(
            invites,
            e => Task.FromResult(new Invite(e.metadata.Code, e.guildId, e.metadata.CreatorId, e.metadata.CreatedAt, e.metadata.Uses))
        );

        return ApiResult.Ok(result);
    }

    private async Task<PaginatedResponse<(string guildId, InviteMetadata metadata)>> ReadCachedInvitesAsync(InviteListRequest request)
    {
        var cursor = 0L;
        var result = new List<(string guildId, InviteMetadata metadata)>();

        do
        {
            var keys = _redisServer.KeysAsync(pattern: "InviteMetadata-*", pageSize: 100, cursor: cursor);
            await foreach (var key in keys)
            {
                var metadata = await ReadCachedInviteWithFilterAsync(key, request);
                if (metadata is not null)
                    result.Add(metadata.Value);
            }

            cursor = ((IScanningCursor)keys).Cursor;
        }
        while (cursor != 0);

        var sortedData = request.Sort.OrderBy?.ToLower() switch
        {
            "created" => request.Sort.Descending ? [.. result.OrderByDescending(o => o.metadata.CreatedAt)] : [.. result.OrderBy(o => o.metadata.CreatedAt)],
            "uses" => request.Sort.Descending ? [.. result.OrderByDescending(o => o.metadata.Uses)] : [.. result.OrderBy(o => o.metadata.Uses)],
            _ => request.Sort.Descending ? [.. result.OrderByDescending(o => o.metadata.Code)] : result.OrderBy(o => o.metadata.Code).ToList()
        };

        return PaginatedResponse<(string guildId, InviteMetadata metadata)>.Create(sortedData, request.Pagination);
    }

    private async Task<(string guildId, InviteMetadata metadata)?> ReadCachedInviteWithFilterAsync(RedisKey key, InviteListRequest request)
    {
        var keyValue = key.ToString();
        var guildId = keyValue.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)[1];

        if (!string.IsNullOrEmpty(request.GuildId) && guildId != request.GuildId)
            return null;
        if (!string.IsNullOrEmpty(request.Code) && !keyValue.EndsWith(request.Code))
            return null;

        var metadata = await _cache.GetAsync<InviteMetadata>(keyValue);
        if (metadata is null)
            return null;

        if (request.OnlyWithoutCreator && metadata.CreatorId is not null)
            return null;
        if (!string.IsNullOrEmpty(request.CreatorId) && metadata.CreatorId != request.CreatorId)
            return null;

        if ((request.CreatedFrom ?? request.CreatedTo) is not null && metadata.CreatedAt is null)
            return null;
        if (request.CreatedFrom is not null && metadata.CreatedAt is not null && !(metadata.CreatedAt >= request.CreatedFrom.Value))
            return null;
        if (request.CreatedTo is not null && metadata.CreatedAt is not null && !(metadata.CreatedAt <= request.CreatedTo.Value))
            return null;

        return (guildId, metadata);
    }
}
