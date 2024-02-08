using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetAuditLogStatisticsAction : ApiActionBase
{
    private AuditLogStatisticsContext StatisticsContext { get; }
    private AuditLogServiceContext AuditLogServiceContext { get; }
    private ICounterManager CounterManager { get; }

    public GetAuditLogStatisticsAction(AuditLogStatisticsContext statisticsContext, AuditLogServiceContext auditLogServiceContext, ICounterManager counterManager)
    {
        StatisticsContext = statisticsContext;
        AuditLogServiceContext = auditLogServiceContext;
        CounterManager = counterManager;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var result = new AuditLogStatistics
        {
            ByDate = await GetStatisticsByDateAsync(),
            ByType = await GetStatisticsByTypeAsync(),
            FileExtensionStatistics = await GetFileExtensionStatisticsAsync()
        };

        return ApiResult.Ok(result);
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

    private async Task<List<FileExtensionStatistic>> GetFileExtensionStatisticsAsync()
    {
        var query = AuditLogServiceContext.Files.AsNoTracking()
            .Where(o => !AuditLogServiceContext.LogItems.Any(x => x.IsDeleted && x.Id == o.LogItemId))
            .GroupBy(o => o.Extension ?? ".NoExtension")
            .Select(o => new FileExtensionStatistic
            {
                Extension = o.Key,
                Count = o.Count(),
                Size = o.Sum(x => x.Size)
            })
            .OrderBy(o => o.Extension);

        using (CounterManager.Create("Api.Statistics.GetAuditLogStatistics.GetFileExtensionStatistics"))
            return await query.ToListAsync();
    }
}
