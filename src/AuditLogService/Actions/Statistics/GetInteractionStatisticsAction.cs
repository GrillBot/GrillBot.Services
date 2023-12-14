using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetInteractionStatisticsAction : ApiActionBase
{
    private AuditLogStatisticsContext StatisticsContext { get; }

    public GetInteractionStatisticsAction(AuditLogStatisticsContext statisticsContext)
    {
        StatisticsContext = statisticsContext;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var statistics = new InteractionStatistics
        {
            ByDate = await GetStatisticsByDateAsync(),
            Commands = await GetCommandStatisticsAsync()
        };

        return ApiResult.Ok(statistics);
    }

    private async Task<Dictionary<string, long>> GetStatisticsByDateAsync()
    {
        return await StatisticsContext.InteractionDateCountStatistics.AsNoTracking()
            .GroupBy(o => new { o.Date.Year, o.Date.Month })
            .OrderBy(o => o.Key.Year).ThenBy(o => o.Key.Month)
            .Select(o => new
            {
                Key = $"{o.Key.Year}-{o.Key.Month.ToString().PadLeft(2, '0')}",
                Count = o.Sum(x => x.Count)
            })
            .ToDictionaryAsync(o => o.Key, o => o.Count);
    }

    private async Task<List<StatisticItem>> GetCommandStatisticsAsync()
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

        return result
            .OrderByDescending(o => o.AvgDuration)
            .ThenByDescending(o => o.SuccessCount + o.FailedCount)
            .ThenBy(o => o.Key)
            .ToList();
    }
}
