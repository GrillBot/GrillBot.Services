using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetAuditLogStatisticsAction : ApiActionBase
{
    private AuditLogStatisticsContext StatisticsContext { get; }

    public GetAuditLogStatisticsAction(AuditLogStatisticsContext statisticsContext)
    {
        StatisticsContext = statisticsContext;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var result = new AuditLogStatistics
        {
            FileCounts = await GetFileStatisticsWithCountAsync(),
            ByDate = await GetStatisticsByDateAsync(),
            ByType = await GetStatisticsByTypeAsync(),
            FileSizes = await GetFileStatisticsWithSizeAsync()
        };

        return ApiResult.FromSuccess(result);
    }

    private async Task<Dictionary<string, long>> GetStatisticsByTypeAsync()
    {
        var data = await StatisticsContext.TypeStatistics.AsNoTracking().ToListAsync();

        return data
            .Select(o => new { Type = o.Type.ToString(), o.Count })
            .OrderBy(o => o.Type)
            .ToDictionary(o => o.Type, o => o.Count);
    }

    private async Task<Dictionary<string, long>> GetStatisticsByDateAsync()
    {
        return await StatisticsContext.DateStatistics.AsNoTracking()
            .GroupBy(o => new { o.Date.Year, o.Date.Month })
            .OrderBy(o => o.Key.Year).ThenBy(o => o.Key.Month)
            .Select(o => new { Date = $"{o.Key.Year}-{o.Key.Month.ToString().PadLeft(2, '0')}", Count = o.Sum(x => x.Count) })
            .ToDictionaryAsync(o => o.Date, o => o.Count);
    }

    private async Task<Dictionary<string, long>> GetFileStatisticsWithCountAsync()
    {
        return await StatisticsContext.FileExtensionStatistics.AsNoTracking()
            .Select(o => new { o.Extension, o.Count })
            .OrderBy(o => o.Extension)
            .ToDictionaryAsync(o => o.Extension, o => o.Count);
    }

    private async Task<Dictionary<string, long>> GetFileStatisticsWithSizeAsync()
    {
        return await StatisticsContext.FileExtensionStatistics.AsNoTracking()
            .Select(o => new { o.Extension, o.Size })
            .OrderBy(o => o.Extension)
            .ToDictionaryAsync(o => o.Extension, o => o.Size);
    }
}
