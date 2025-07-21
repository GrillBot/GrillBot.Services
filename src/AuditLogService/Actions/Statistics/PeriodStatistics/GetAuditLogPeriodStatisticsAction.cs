using AuditLogService.Core.Entity;
using GrillBot.Services.Common.Actions.Statistics;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics.PeriodStatistics;

public class GetAuditLogPeriodStatisticsAction(IServiceProvider serviceProvider) : PeriodStatisticsActionBase<AuditLogServiceContext>(serviceProvider)
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
