using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetInteractionStatisticsAction(
    AuditLogStatisticsContext _statisticsContext,
    AuditLogServiceContext dbContext,
    ICounterManager counterManager
) : ApiAction<AuditLogServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var statistics = new InteractionStatistics
        {
            Commands = await GetCommandStatisticsAsync()
        };

        return ApiResult.Ok(statistics);
    }

    private async Task<List<StatisticItem>> GetCommandStatisticsAsync()
    {
        var statsQuery = _statisticsContext.InteractionStatistics.AsNoTracking().Select(o => new StatisticItem
        {
            FailedCount = o.FailedCount,
            Key = o.Action,
            Last = o.LastRun,
            LastRunDuration = o.LastRunDuration,
            MaxDuration = o.MaxDuration,
            MinDuration = o.MinDuration,
            SuccessCount = o.SuccessCount,
            TotalDuration = o.TotalDuration
        });

        var stats = await ContextHelper.ReadEntitiesAsync(statsQuery);
        return [.. stats
            .OrderByDescending(o => o.AvgDuration)
            .ThenByDescending(o => o.SuccessCount + o.FailedCount)
            .ThenBy(o => o.Key)
        ];
    }
}
