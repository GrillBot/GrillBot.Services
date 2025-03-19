using AuditLogService.Core.Entity.Statistics;
using GrillBot.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Core.Providers;

public class StatisticsProvider(AuditLogStatisticsContext _statisticsContext) : IStatisticsProvider
{
    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
    {
        return await _statisticsContext.DatabaseStatistics.AsNoTracking()
            .OrderBy(o => o.TableName)
            .Select(o => new { o.TableName, o.RecordsCount })
            .ToDictionaryAsync(o => o.TableName, o => o.RecordsCount);
    }
}
