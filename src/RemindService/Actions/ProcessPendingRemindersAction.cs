using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Services.Common.Infrastructure.Api;
using RemindService.Core.Entity;
using RemindService.Models.Events;
using RemindService.Models.Response;

namespace RemindService.Actions;

public class ProcessPendingRemindersAction : ApiAction<RemindServiceContext>
{
    private readonly IRabbitMQPublisher _publisher;

    public ProcessPendingRemindersAction(ICounterManager counterManager, RemindServiceContext dbContext, IRabbitMQPublisher publisher) : base(counterManager, dbContext)
    {
        _publisher = publisher;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var now = DateTime.UtcNow;
        var query = DbContext.RemindMessages
            .Where(o => !o.IsSendInProgress && string.IsNullOrEmpty(o.NotificationMessageId) && o.NotifyAtUtc <= now)
            .OrderBy(o => o.Id);

        var pendingMessages = await ContextHelper.ReadEntitiesAsync(query);
        var report = new List<string>();

        foreach (var message in pendingMessages)
        {
            AddRemindToReport(message, report);
            await _publisher.PublishAsync(new SendRemindNotificationPayload(message.Id, false));
        }

        var result = new ProcessPendingRemindersResult(pendingMessages.Count, report);
        return ApiResult.Ok(result);
    }
    private static void AddRemindToReport(RemindMessage message, List<string> report)
    {
        var msgFields = new List<string>
        {
            $"Id: {message.Id}",
            $"FromUserId: {message.FromUserId}",
            $"ToUserId: {message.ToUserId}",
            $"MessageLength: {message.Message.Length}",
            $"Language: {message.Language}",
            $"PostponeCount: {message.PostponeCount}"
        };

        report.Add(string.Join(", ", msgFields));
    }
}
