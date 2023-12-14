using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetUserCommandStatisticsAction : ApiActionBase
{
    private AuditLogStatisticsContext StatisticsContext { get; }

    public GetUserCommandStatisticsAction(AuditLogStatisticsContext statisticsContext)
    {
        StatisticsContext = statisticsContext;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var data = await StatisticsContext.InteractionUserActionStatistics.AsNoTracking().ToListAsync();
        var result = data.ConvertAll(o => new UserActionCountItem
        {
            Action = o.Action,
            Count = o.Count,
            UserId = o.UserId!
        });

        return ApiResult.Ok(result);
    }
}
