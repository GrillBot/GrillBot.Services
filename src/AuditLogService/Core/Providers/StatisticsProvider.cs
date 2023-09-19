using AuditLogService.Core.Entity.Statistics;
using GrillBot.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Core.Providers;

public class StatisticsProvider : IStatisticsProvider
{
    private AuditLogStatisticsContext StatisticsContext { get; }

    public StatisticsProvider(AuditLogStatisticsContext statisticsContext)
    {
        StatisticsContext = statisticsContext;
    }

    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
    {
        return await StatisticsContext.DatabaseStatistics.AsNoTracking()
            .OrderBy(o => o.TableName)
            .Select(o => new { o.TableName, o.RecordsCount })
            .ToDictionaryAsync(o => o.TableName, o => o.RecordsCount);
    }
}
