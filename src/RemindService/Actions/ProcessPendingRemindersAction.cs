using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Services.Common.Infrastructure.Api;
using RemindService.Core.Entity;
using RemindService.Models.Events;
using RemindService.Models.Response;

namespace RemindService.Actions;

public class ProcessPendingRemindersAction(
    ICounterManager counterManager,
    RemindServiceContext dbContext,
    IRabbitPublisher publisher
) : ApiAction<RemindServiceContext>(counterManager, dbContext)
{
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

            var payload = new SendRemindNotificationPayload(message.Id, false);
            await publisher.PublishAsync(payload);
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
