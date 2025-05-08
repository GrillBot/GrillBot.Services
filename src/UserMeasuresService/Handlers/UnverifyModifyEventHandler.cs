using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using UserMeasuresService.Handlers.Abstractions;
using UserMeasuresService.Models.Events;

namespace UserMeasuresService.Handlers;

public class UnverifyModifyEventHandler(
    IServiceProvider serviceProvider
) : BaseMeasuresHandler<UnverifyModifyPayload>(serviceProvider)
{
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
