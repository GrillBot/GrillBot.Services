using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using RubbergodService.Core.Entity;
using RubbergodService.Models.Events.Pins;

namespace RubbergodService.Handlers.Pins;

public class ClearPinCacheEventHandler : BaseEventHandlerWithDb<ClearPinCachePayload, RubbergodServiceContext>
{
    public ClearPinCacheEventHandler(ILoggerFactory loggerFactory, RubbergodServiceContext dbContext, ICounterManager counterManager, IRabbitMQPublisher publisher)
        : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

    protected override async Task HandleInternalAsync(ClearPinCachePayload payload, Dictionary<string, string> headers)
    {
        var query = DbContext.PinCache.Where(o => o.GuildId == payload.GuildId && o.ChannelId == payload.ChannelId);
        await ContextHelper.ExecuteBatchDeleteAsync(query);
    }
}
