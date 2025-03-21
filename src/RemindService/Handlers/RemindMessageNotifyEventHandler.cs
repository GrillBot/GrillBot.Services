﻿using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using RemindService.Core.Entity;
using RemindService.Models.Events;

namespace RemindService.Handlers;

public class RemindMessageNotifyEventHandler(ILoggerFactory loggerFactory,
    RemindServiceContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher publisher
) : BaseEventHandlerWithDb<RemindMessageNotifyPayload, RemindServiceContext>(loggerFactory, dbContext, counterManager, publisher)
{
    public override string TopicName => "Remind";
    public override string QueueName => "RemindNotified";

    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(RemindMessageNotifyPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        var messageQuery = DbContext.RemindMessages.Where(o => o.Id == message.RemindId);
        var remindMessage = await ContextHelper.ReadFirstOrDefaultEntityAsync(messageQuery);
        if (remindMessage is null)
            return RabbitConsumptionResult.Success;

        remindMessage.IsSendInProgress = false;
        remindMessage.NotificationMessageId = message.NotificationMessageId;

        await ContextHelper.SaveChagesAsync();
        return RabbitConsumptionResult.Success;
    }
}
