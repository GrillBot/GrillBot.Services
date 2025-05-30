using AuditLogService.Core.Entity;
using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics.PeriodStatistics;

public class GetAuditLogPeriodStatisticsAction(
    AuditLogServiceContext dbContext,
    ICounterManager counterManager
) : PeriodStatisticsActionBase(dbContext, counterManager)
{
    protected override async Task<Dictionary<DateOnly, long>> GetRawDataAsync()
    {
        var query = DbContext.LogItems.AsNoTracking()
            .GroupBy(o => o.LogDate)
            .Select(o => new
            {
                o.Key,
                Count = o.LongCount()
            });

        return await ContextHelper.ReadToDictionaryAsync(query, o => o.Key, o => o.Count);
    }
}
