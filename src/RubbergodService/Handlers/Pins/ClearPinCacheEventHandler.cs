using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using Microsoft.Extensions.Caching.Distributed;
using RubbergodService.Models.Events.Pins;

namespace RubbergodService.Handlers.Pins;

public class ClearPinCacheEventHandler(
    ILoggerFactory loggerFactory,
    ICounterManager counterManager,
    IRabbitPublisher publisher,
    IDistributedCache _cache
) : BaseEventHandler<ClearPinCachePayload>(loggerFactory, counterManager, publisher)
{
    public override string TopicName => "Rubbergod";
    public override string QueueName => "ClearPinCache";

    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(ClearPinCachePayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
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
