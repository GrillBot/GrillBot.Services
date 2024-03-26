using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetInteractionStatisticsAction : ApiAction<AuditLogServiceContext>
{
    private AuditLogStatisticsContext StatisticsContext { get; }

    public GetInteractionStatisticsAction(AuditLogStatisticsContext statisticsContext, AuditLogServiceContext dbContext, ICounterManager counterManager)
        : base(counterManager, dbContext)
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
        var statsQuery = DbContext.InteractionCommands.AsNoTracking()
            .GroupBy(o => o.InteractionDate)
            .Select(o => new { o.Key, Count = o.LongCount() });
        var stats = await ContextHelper.ReadEntitiesAsync(statsQuery);

        return stats
            .GroupBy(o => new { o.Key.Year, o.Key.Month })
            .OrderBy(o => o.Key.Year).ThenBy(o => o.Key.Month)
            .ToDictionary(o => $"{o.Key.Year}-{o.Key.Month.ToString().PadLeft(2, '0')}", o => o.Sum(x => x.Count));
    }

    private async Task<List<StatisticItem>> GetCommandStatisticsAsync()
    {
        var statsQuery = StatisticsContext.InteractionStatistics.AsNoTracking().Select(o => new StatisticItem
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
        return stats
            .OrderByDescending(o => o.AvgDuration)
            .ThenByDescending(o => o.SuccessCount + o.FailedCount)
            .ThenBy(o => o.Key)
            .ToList();
    }
}
