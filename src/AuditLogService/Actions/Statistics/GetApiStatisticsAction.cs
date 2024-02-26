using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetApiStatisticsAction : ApiAction
{
    private AuditLogStatisticsContext StatisticsContext { get; }
    private AuditLogServiceContext DbContext { get; }

    public GetApiStatisticsAction(AuditLogStatisticsContext statisticsContext, AuditLogServiceContext dbContext,
        ICounterManager counterManager) : base(counterManager)
    {
        StatisticsContext = statisticsContext;
        DbContext = dbContext;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var statistics = new ApiStatistics
        {
            ByDateInternalApi = await GetApiStatisticsByDateForApiGroupAsync("V1", "V3"),
            ByDatePublicApi = await GetApiStatisticsByDateForApiGroupAsync("V2"),
            Endpoints = await GetEndpointStatisticsAsync()
        };

        return ApiResult.Ok(statistics);
    }

    private async Task<Dictionary<string, long>> GetApiStatisticsByDateForApiGroupAsync(params string[] apiGroupNames)
    {
        using (CreateCounter($"Database"))
        {
            var stats = await DbContext.ApiRequests.AsNoTracking()
                .Where(o => apiGroupNames.Contains(o.ApiGroupName))
                .GroupBy(o => o.RequestDate)
                .Select(o => new { o.Key, Count = o.LongCount() })
                .ToListAsync();

            return stats
                .GroupBy(o => new { o.Key.Year, o.Key.Month })
                .OrderBy(o => o.Key.Year).ThenBy(o => o.Key.Month)
                .ToDictionary(o => $"{o.Key.Year}-{o.Key.Month.ToString().PadLeft(2, '0')}", o => o.Sum(x => x.Count));
        }
    }

    private async Task<List<StatisticItem>> GetEndpointStatisticsAsync()
    {
        using (CreateCounter("Database"))
        {
            var stats = await StatisticsContext.RequestStats.AsNoTracking()
                .Select(o => new StatisticItem
                {
                    FailedCount = o.FailedCount,
                    Key = o.Endpoint,
                    Last = o.LastRequest,
                    LastRunDuration = o.LastRunDuration,
                    MaxDuration = o.MaxDuration,
                    MinDuration = o.MinDuration,
                    SuccessCount = o.SuccessCount,
                    TotalDuration = o.TotalDuration
                })
                .ToListAsync();

            return stats
                .OrderByDescending(o => o.AvgDuration)
                .ThenBy(o => o.Key)
                .ToList();
        }
    }
}
