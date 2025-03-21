using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using RemindService.Core.Entity;
using RemindService.Models.Request;
using RemindService.Models.Response;

namespace RemindService.Actions;

public class CreateReminderAction(
    ICounterManager counterManager,
    RemindServiceContext dbContext
) : ApiAction<RemindServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<CreateReminderRequest>(0);

        var reminder = new RemindMessage
        {
            CommandMessageId = request.CommandMessageId,
            FromUserId = request.FromUserId,
            IsSendInProgress = false,
            Language = request.Language,
            Message = request.Message,
            NotificationMessageId = null,
            NotifyAtUtc = request.NotifyAtUtc,
            PostponeCount = 0,
            ToUserId = request.ToUserId
        };

        await DbContext.AddAsync(reminder);
        await ContextHelper.SaveChagesAsync();

        var result = new CreateReminderResult(reminder.Id);
        return ApiResult.Ok(result);
    }
}
