using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Handlers.Abstractions;
using UserMeasuresService.Models.Events;

namespace UserMeasuresService.Handlers;

public class UnverifyModifyEventHandler : BaseMeasuresHandler<UnverifyModifyPayload>
{
    public UnverifyModifyEventHandler(ILoggerFactory loggerFactory, UserMeasuresContext dbContext, ICounterManager counterManager, IRabbitMQPublisher publisher)
        : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

    protected override async Task HandleInternalAsync(UnverifyModifyPayload payload, Dictionary<string, string> headers)
    {
        var item = await ContextHelper.ReadFirstOrDefaultEntityAsync(DbContext.Unverifies.Where(o => o.LogSetId == payload.LogSetId));
        if (item is null)
            return;

        if (payload.NewEndUtc.HasValue)
            item.ValidTo = payload.NewEndUtc.Value.ToUniversalTime();

        await SaveEntityAsync(item);
    }
}
