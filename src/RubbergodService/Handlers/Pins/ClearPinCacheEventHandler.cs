using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using Microsoft.Extensions.Caching.Distributed;
using RubbergodService.Models.Events.Pins;

namespace RubbergodService.Handlers.Pins;

public class ClearPinCacheEventHandler : BaseEventHandler<ClearPinCachePayload>
{
    private readonly IDistributedCache _cache;

    public ClearPinCacheEventHandler(ILoggerFactory loggerFactory, ICounterManager counterManager, IRabbitMQPublisher publisher, IDistributedCache cache)
        : base(loggerFactory, counterManager, publisher)
    {
        _cache = cache;
    }

    protected override async Task HandleInternalAsync(ClearPinCachePayload payload, Dictionary<string, string> headers)
    {
        await RemoveItemAsync("md", payload);
        await RemoveItemAsync("json", payload);
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
