using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.EntityFrameworkCore;
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

    protected override async Task HandleInternalAsync(UnverifyModifyPayload payload)
    {
        var item = await DbContext.Unverifies.FirstOrDefaultAsync(o => o.LogSetId == payload.LogSetId);
        if (item is null)
            return;

        if (payload.NewEnd.HasValue)
            item.ValidTo = payload.NewEnd.Value.ToUniversalTime();

        await SaveEntityAsync(item);
    }
}
