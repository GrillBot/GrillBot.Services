using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using RemindService.Core.Entity;
using RemindService.Models.Events;

namespace RemindService.Handlers;

public class RemindMessageNotifyEventHandler(
    IServiceProvider serviceProvider
) : BaseEventHandlerWithDb<RemindMessageNotifyPayload, RemindServiceContext>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        RemindMessageNotifyPayload message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        var messageQuery = DbContext.RemindMessages.Where(o => o.Id == message.RemindId);
        var remindMessage = await ContextHelper.ReadFirstOrDefaultEntityAsync(messageQuery, cancellationToken);
        if (remindMessage is null)
            return RabbitConsumptionResult.Success;

        remindMessage.IsSendInProgress = false;
        remindMessage.NotificationMessageId = message.NotificationMessageId;

        await ContextHelper.SaveChangesAsync(cancellationToken);
        return RabbitConsumptionResult.Success;
    }
}
