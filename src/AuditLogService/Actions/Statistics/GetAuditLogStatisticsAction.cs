using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetAuditLogStatisticsAction : ApiAction
{
    private AuditLogStatisticsContext StatisticsContext { get; }
    private AuditLogServiceContext DbContext { get; }

    public GetAuditLogStatisticsAction(AuditLogStatisticsContext statisticsContext, AuditLogServiceContext auditLogServiceContext,
        ICounterManager counterManager) : base(counterManager)
    {
        StatisticsContext = statisticsContext;
        DbContext = auditLogServiceContext;
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
        using (CreateCounter("GetStatisticsByType"))
        {
            var stats = await DbContext.LogItems.AsNoTracking()
                .GroupBy(o => o.Type)
                .OrderBy(o => o.Key)
                .Select(o => new { o.Key, Count = o.LongCount() })
                .ToListAsync();

            return stats.ToDictionary(o => o.Key.ToString(), o => o.Count);
        }
    }

    private async Task<Dictionary<string, long>> GetStatisticsByDateAsync()
    {
        using (CreateCounter("GetStatisticsByDate"))
        {
            var stats = await DbContext.LogItems.AsNoTracking()
                .GroupBy(o => o.LogDate)
                .Select(o => new { o.Key, Count = o.LongCount() })
                .ToListAsync();

            return stats
                .GroupBy(o => new { o.Key.Year, o.Key.Month })
                .OrderBy(o => o.Key.Year).ThenBy(o => o.Key.Month)
                .ToDictionary(o => $"{o.Key.Year}-{o.Key.Month.ToString().PadLeft(2, '0')}", o => o.Sum(x => x.Count));
        }
    }

    private async Task<List<FileExtensionStatistic>> GetFileExtensionStatisticsAsync()
    {
        using (CreateCounter("GetFileExtensionStatistics"))
        {
            var query = DbContext.Files.AsNoTracking()
                .GroupBy(o => (o.Extension ?? ".NoExtension").ToLower())
                .Select(o => new FileExtensionStatistic
                {
                    Extension = o.Key,
                    Count = o.Count(),
                    Size = o.Sum(x => x.Size)
                })
                .OrderBy(o => o.Extension);

            return await query.ToListAsync();
        }
    }
}
