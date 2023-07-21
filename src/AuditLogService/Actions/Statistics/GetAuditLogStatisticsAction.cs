using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetAuditLogStatisticsAction : ApiActionBase
{
    private AuditLogServiceContext Context { get; }
    private AuditLogStatisticsContext StatisticsContext { get; }

    public GetAuditLogStatisticsAction(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext)
    {
        Context = context;
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

    private async Task<Dictionary<string, int>> GetStatisticsByDateAsync()
    {
        return await Context.LogItems.AsNoTracking()
            .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
            .OrderBy(o => o.Key.Year).ThenBy(o => o.Key.Month)
            .Select(o => new { Date = $"{o.Key.Year}-{o.Key.Month.ToString().PadLeft(2, '0')}", Count = o.Count() })
            .ToDictionaryAsync(o => o.Date, o => o.Count);
    }

    private async Task<Dictionary<string, int>> GetFileStatisticsWithCountAsync()
    {
        return await Context.Files.AsNoTracking()
            .GroupBy(o => o.Extension)
            .Select(o => new { Key = o.Key ?? ".Noextension", Count = o.Count() })
            .OrderBy(o => o.Key)
            .ToDictionaryAsync(o => o.Key, o => o.Count);
    }

    private async Task<Dictionary<string, long>> GetFileStatisticsWithSizeAsync()
    {
        return await Context.Files.AsNoTracking()
            .GroupBy(o => o.Extension)
            .Select(o => new { Key = o.Key ?? ".NoExtension", Size = o.Sum(x => x.Size) })
            .OrderBy(o => o.Key)
            .ToDictionaryAsync(o => o.Key, o => o.Size);
    }
}
