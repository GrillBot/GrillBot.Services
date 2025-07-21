using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using Microsoft.Extensions.Caching.Distributed;
using RubbergodService.Models.Events.Pins;

namespace RubbergodService.Handlers.Pins;

public class ClearPinCacheEventHandler(
    IServiceProvider serviceProvider,
    IDistributedCache _cache
) : BaseEventHandler<ClearPinCachePayload>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        ClearPinCachePayload message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        await RemoveItemAsync("md", message);
        await RemoveItemAsync("json", message);
        return RabbitConsumptionResult.Success;
    }

    private async Task RemoveItemAsync(string type, ClearPinCachePayload payload)
    {
        var cacheKey = $"RubbergodService/PinCacheItem({payload.GuildId}, {payload.ChannelId}, {type})";

        byte[]? cacheItem;
        using (CreateCounter("Redis"))
            cacheItem = await _cache.GetAsync(cacheKey);

        if (cacheItem is null)
            return;

        using (CreateCounter("Redis"))
            await _cache.RemoveAsync(cacheKey);
    }
}
