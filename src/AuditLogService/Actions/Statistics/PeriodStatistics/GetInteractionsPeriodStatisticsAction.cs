using AuditLogService.Core.Entity;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Actions.Statistics;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics.PeriodStatistics;

public class GetInteractionsPeriodStatisticsAction(
    AuditLogServiceContext dbContext,
    ICounterManager counterManager
) : PeriodStatisticsActionBase<AuditLogServiceContext>(dbContext, counterManager)
{
    protected override async Task<Dictionary<DateOnly, long>> GetRawDataAsync()
    {
        var query = DbContext.InteractionCommands.AsNoTracking()
            .GroupBy(o => o.InteractionDate)
            .Select(o => new
            {
                o.Key,
                Count = o.LongCount()
            });

        return await ContextHelper.ReadToDictionaryAsync(query, o => o.Key, o => o.Count);
    }
}
