using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using RemindService.Core.Entity;
using RemindService.Models.Response;

namespace RemindService.Actions;

public class ReminderSuggestionsAction : ApiAction<RemindServiceContext>
{
    public ReminderSuggestionsAction(ICounterManager counterManager, RemindServiceContext dbContext) : base(counterManager, dbContext)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var userId = GetParameter<string>(0);

        var query = DbContext.RemindMessages
            .AsNoTracking()
            .Where(o => (o.FromUserId == userId || o.ToUserId == userId) && o.NotificationMessageId == null && !o.IsSendInProgress)
            .OrderByDescending(o => o.NotifyAtUtc)
            .Take(25)
            .Select(o => new ReminderSuggestionItem(o.Id, o.FromUserId, o.ToUserId, o.ToUserId == userId, o.NotifyAtUtc));

        var result = await ContextHelper.ReadEntitiesAsync(query);
        return ApiResult.Ok(result);
    }
}
