using Discord;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Core.Services.AuditLog.Enums;
using GrillBot.Core.Services.AuditLog.Models.Events.Create;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using RemindService.Core.Entity;
using RemindService.Models.Events;
using RemindService.Models.Request;
using RemindService.Options;

namespace RemindService.Actions;

public class CancelReminderAction : ApiAction<RemindServiceContext>
{
    private readonly IRabbitMQPublisher _publisher;

    public CancelReminderAction(ICounterManager counterManager, RemindServiceContext dbContext, IRabbitMQPublisher publisher) : base(counterManager, dbContext)
    {
        _publisher = publisher;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<CancelReminderRequest>(0);

        var remind = await ContextHelper.ReadFirstOrDefaultEntityAsync(DbContext.RemindMessages.Where(o => o.Id == request.RemindId));
        var modelState = CheckRemindStatus(remind, request);
        if (!modelState.IsValid)
            return ApiResult.BadRequest(new ValidationProblemDetails(modelState));

        await WriteToAuditLogAsync(remind!, request);
        await NotifyUserIfPossibleAsync(remind!, request);
        await ContextHelper.SaveChagesAsync();

        return ApiResult.Ok();
    }

    private static ModelStateDictionary CheckRemindStatus(RemindMessage? message, CancelReminderRequest request)
    {
        var modelState = new ModelStateDictionary();

        if (message is null)
        {
            modelState.AddModelError(nameof(request.RemindId), "RemindModule/CancelRemind/NotFound");
            return modelState;
        }

        if (!string.IsNullOrEmpty(message.NotificationMessageId))
            modelState.AddModelError("Remind", "RemindModule/CancelRemind/AlreadyNotified");

        if (message.NotificationMessageId == AppOptions.FinishedUnsentMessageId)
            modelState.AddModelError("Remind", "RemindModule/CancelRemind/AlreadyCancelled");

        if (message.IsSendInProgress)
            modelState.AddModelError("Remind", "RemindModule/RemindInProgress");

        if (message.FromUserId != request.ExecutingUserId && message.ToUserId != request.ExecutingUserId && !request.IsAdminExecution)
            modelState.AddModelError("Remind", "RemindModule/CancelRemind/InvalidOperator");

        return modelState;
    }

    private async Task NotifyUserIfPossibleAsync(RemindMessage message, CancelReminderRequest request)
    {
        if (!request.NotifyUser)
        {
            message.IsSendInProgress = false;
            message.NotificationMessageId = AppOptions.FinishedUnsentMessageId;
            return;
        }

        var payload = new SendRemindNotificationPayload(request.RemindId, true);
        await _publisher.PublishAsync(payload);
    }

    private async Task WriteToAuditLogAsync(RemindMessage message, CancelReminderRequest request)
    {
        var messageTemplate = request.NotifyUser ?
            "The reminder with ID {0} has been canceled. A notification was sent to the user upon cancellation." :
            "The reminder with ID {0} has been canceled.";

        var logRequest = new LogRequest(LogType.Info, DateTime.UtcNow, null, request.ExecutingUserId)
        {
            LogMessage = new LogMessageRequest(string.Format(messageTemplate, message.Id), LogSeverity.Info, "RemindService", nameof(CancelReminderAction))
        };

        await _publisher.PublishAsync(new CreateItemsPayload(logRequest));
    }
}
