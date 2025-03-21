using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Handlers.Abstractions;
using UserMeasuresService.Models.Events;

namespace UserMeasuresService.Handlers;

public class UnverifyModifyEventHandler(
    ILoggerFactory loggerFactory,
    UserMeasuresContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher publisher
) : BaseMeasuresHandler<UnverifyModifyPayload>(loggerFactory, dbContext, counterManager, publisher)
{
    public override string QueueName => "ModifyUnverify";

    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(UnverifyModifyPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        var item = await ContextHelper.ReadFirstOrDefaultEntityAsync(DbContext.Unverifies.Where(o => o.LogSetId == message.LogSetId));
        if (item is null)
            return RabbitConsumptionResult.Success;

        if (message.NewEndUtc.HasValue)
            item.ValidTo = message.NewEndUtc.Value.ToUniversalTime();

        await SaveEntityAsync(item);
        return RabbitConsumptionResult.Success;
    }
}
