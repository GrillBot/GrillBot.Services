using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Handlers.Abstractions;
using UserMeasuresService.Models.Events;

namespace UserMeasuresService.Handlers;

public class UnverifyModifyEventHandler : BaseEventHandlerWithDb<UnverifyModifyPayload>
{
    public override string QueueName => new UnverifyModifyPayload().QueueName;

    public UnverifyModifyEventHandler(ILoggerFactory loggerFactory, UserMeasuresContext dbContext, ICounterManager counterManager)
        : base(loggerFactory, dbContext, counterManager)
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
