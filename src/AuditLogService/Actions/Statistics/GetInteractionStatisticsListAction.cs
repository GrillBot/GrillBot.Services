using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetInteractionStatisticsListAction : ApiActionBase
{
    private AuditLogStatisticsContext StatisticsContext { get; }

    public GetInteractionStatisticsListAction(AuditLogStatisticsContext statisticsContext)
    {
        StatisticsContext = statisticsContext;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var result = await StatisticsContext.InteractionStatistics.AsNoTracking()
            .Select(o => new StatisticItem
            {
                FailedCount = o.FailedCount,
                Key = o.Action,
                Last = o.LastRun,
                LastRunDuration = o.LastRunDuration,
                MaxDuration = o.MaxDuration,
                MinDuration = o.MinDuration,
                SuccessCount = o.SuccessCount,
                TotalDuration = o.TotalDuration
            })
            .ToListAsync();

        result = result
            .OrderByDescending(o => o.AvgDuration)
            .ThenByDescending(o => o.SuccessCount + o.FailedCount)
            .ThenBy(o => o.Key)
            .ToList();

        return ApiResult.FromSuccess(result);
    }
}
