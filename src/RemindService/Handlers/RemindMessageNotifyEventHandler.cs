using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using RemindService.Core.Entity;
using RemindService.Models.Events;

namespace RemindService.Handlers;

public class RemindMessageNotifyEventHandler : BaseEventHandlerWithDb<RemindMessageNotifyPayload, RemindServiceContext>
{
    public RemindMessageNotifyEventHandler(ILoggerFactory loggerFactory, RemindServiceContext dbContext,
        ICounterManager counterManager, IRabbitMQPublisher publisher) : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

    protected override async Task HandleInternalAsync(RemindMessageNotifyPayload payload, Dictionary<string, string> headers)
    {
        var messageQuery = DbContext.RemindMessages.Where(o => o.Id == payload.RemindId);
        var remindMessage = await ContextHelper.ReadFirstOrDefaultEntityAsync(messageQuery);
        if (remindMessage is null)
            return;

        remindMessage.IsSendInProgress = false;
        remindMessage.NotificationMessageId = payload.NotificationMessageId;

        await ContextHelper.SaveChagesAsync();
    }
}
