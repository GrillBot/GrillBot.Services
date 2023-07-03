using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetAuditLogStatisticsAction : ApiActionBase
{
    private AuditLogServiceContext Context { get; }

    public GetAuditLogStatisticsAction(AuditLogServiceContext context)
    {
        Context = context;
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

    private async Task<Dictionary<string, int>> GetStatisticsByTypeAsync()
    {
        var statistics = await Context.LogItems.AsNoTracking()
            .GroupBy(o => o.Type)
            .Select(o => new { o.Key, Count = o.Count() })
            .ToDictionaryAsync(o => o.Key, o => o.Count);

        return Enum.GetValues<LogType>()
            .Where(o => o != LogType.None)
            .Select(o => new { Key = o.ToString(), Value = statistics.TryGetValue(o, out var value) ? value : 0 })
            .OrderBy(o => o.Key)
            .ToDictionary(o => o.Key, o => o.Value);
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
