using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetApiStatisticsAction : ApiActionBase
{
    private AuditLogStatisticsContext StatisticsContext { get; }

    public GetApiStatisticsAction(AuditLogStatisticsContext statisticsContext)
    {
        StatisticsContext = statisticsContext;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var statistics = new ApiStatistics
        {
            ByDateInternalApi = await GetApiStatisticsByDateForApiGroupAsync("V1"),
            ByDatePublicApi = await GetApiStatisticsByDateForApiGroupAsync("V2"),
            ByStatusCodeInternalApi = await GetApiStatisticsByResultForApiGroupAsync("V1"),
            ByStatusCodePublicApi = await GetApiStatisticsByResultForApiGroupAsync("V2"),
            Endpoints = await GetEndpointStatisticsAsync()
        };

        return ApiResult.FromSuccess(statistics);
    }

    private async Task<Dictionary<string, long>> GetApiStatisticsByDateForApiGroupAsync(string apiGroupName)
    {
        return await StatisticsContext.DateCountStatistics.AsNoTracking()
            .Where(o => o.ApiGroup == apiGroupName)
            .GroupBy(o => new { o.Date.Year, o.Date.Month })
            .OrderBy(o => o.Key.Year).ThenBy(o => o.Key.Month)
            .Select(o => new { Key = $"{o.Key.Year}-{o.Key.Month.ToString().PadLeft(2, '0')}", Count = o.LongCount() })
            .ToDictionaryAsync(o => o.Key, o => o.Count);
    }

    private async Task<Dictionary<string, long>> GetApiStatisticsByResultForApiGroupAsync(string apiGroupName)
    {
        return await StatisticsContext.ResultCountStatistic.AsNoTracking()
            .Where(o => o.ApiGroup == apiGroupName)
            .OrderBy(o => o.Result)
            .Select(o => new { o.Result, o.Count })
            .ToDictionaryAsync(o => o.Result, o => o.Count);
    }

    private async Task<List<StatisticItem>> GetEndpointStatisticsAsync()
    {
        var items = await StatisticsContext.RequestStats.AsNoTracking()
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

        return items
            .OrderByDescending(o => o.AvgDuration)
            .ThenBy(o => o.Key)
            .ToList();
    }
}
