using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using RemindService.Core.Entity;

namespace RemindService.Actions;

public class PostponeRemindAction(
    ICounterManager counterManager,
    RemindServiceContext dbContext
) : ApiAction<RemindServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var notificationMessageId = GetParameter<string>(0);
        var hours = GetParameter<int>(1);

        var remind = await ContextHelper.ReadFirstOrDefaultEntityAsync<RemindMessage>(o => o.NotificationMessageId == notificationMessageId);
        if (remind is null)
            return ApiResult.NotFound();

        remind.IsSendInProgress = false;
        remind.NotificationMessageId = null;
        remind.NotifyAtUtc = DateTime.UtcNow.AddHours(hours);
        remind.PostponeCount++;

        await ContextHelper.SaveChagesAsync();
        return ApiResult.Ok();
    }
}
