using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetApiStatisticsAction : ApiAction<AuditLogServiceContext>
{
    private AuditLogStatisticsContext StatisticsContext { get; }

    public GetApiStatisticsAction(AuditLogStatisticsContext statisticsContext, AuditLogServiceContext dbContext,
        ICounterManager counterManager) : base(counterManager, dbContext)
    {
        StatisticsContext = statisticsContext;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var statistics = new ApiStatistics
        {
            ByDateInternalApi = await GetApiStatisticsByDateForApiGroupAsync("V1", "V3"),
            ByDatePublicApi = await GetApiStatisticsByDateForApiGroupAsync("V2"),
            Endpoints = await GetEndpointStatisticsAsync(),
            DailyInternalApi = await GetDailyApiStatisticsAsync("V1", "V3"),
            DailyPublicApi = await GetDailyApiStatisticsAsync("V2")
        };

        return ApiResult.Ok(statistics);
    }

    private async Task<Dictionary<string, long>> GetApiStatisticsByDateForApiGroupAsync(params string[] apiGroupNames)
    {
        var query = DbContext.ApiRequests.AsNoTracking()
            .Where(o => apiGroupNames.Contains(o.ApiGroupName))
            .GroupBy(o => o.RequestDate)
            .Select(o => new { o.Key, Count = o.LongCount() });
        var stats = await ContextHelper.ReadEntitiesAsync(query);

        return stats
            .GroupBy(o => new { o.Key.Year, o.Key.Month })
            .OrderBy(o => o.Key.Year).ThenBy(o => o.Key.Month)
            .ToDictionary(o => $"{o.Key.Year}-{o.Key.Month.ToString().PadLeft(2, '0')}", o => o.Sum(x => x.Count));
    }

    private async Task<Dictionary<DateOnly, int>> GetDailyApiStatisticsAsync(params string[] apiGroupNames)
    {
        var query = DbContext.ApiRequests.AsNoTracking();

        query = apiGroupNames.Length == 1
            ? query.Where(r => r.ApiGroupName == apiGroupNames[0])
            : query.Where(r => apiGroupNames.Contains(r.ApiGroupName));

        var groupedQuery = query
            .GroupBy(o => o.RequestDate)
            .Select(o => new { o.Key, Count = o.Count() })
            .OrderBy(o => o.Key).ThenBy(o => o.Count);

        return await ContextHelper.ReadToDictionaryAsync(groupedQuery, e => e.Key, e => e.Count);
    }

    private async Task<List<StatisticItem>> GetEndpointStatisticsAsync()
    {
        var query = StatisticsContext.RequestStats.AsNoTracking().Select(o => new StatisticItem
        {
            FailedCount = o.FailedCount,
            Key = o.Endpoint,
            Last = o.LastRequest,
            LastRunDuration = o.LastRunDuration,
            MaxDuration = o.MaxDuration,
            MinDuration = o.MinDuration,
            SuccessCount = o.SuccessCount,
            TotalDuration = o.TotalDuration
        });

        var stats = await ContextHelper.ReadEntitiesAsync(query);
        return stats
            .OrderByDescending(o => o.AvgDuration)
            .ThenBy(o => o.Key)
            .ToList();
    }
}
