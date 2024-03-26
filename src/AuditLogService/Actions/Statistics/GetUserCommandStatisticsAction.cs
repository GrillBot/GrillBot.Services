using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetUserCommandStatisticsAction : ApiAction<AuditLogStatisticsContext>
{
    public GetUserCommandStatisticsAction(AuditLogStatisticsContext statisticsContext, ICounterManager counterManager) : base(counterManager, statisticsContext)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var data = await ContextHelper.ReadEntitiesAsync(DbContext.InteractionUserActionStatistics.AsNoTracking());
        var result = data.ConvertAll(o => new UserActionCountItem
        {
            Action = o.Action,
            Count = o.Count,
            UserId = o.UserId!
        });

        return ApiResult.Ok(result);
    }
}
