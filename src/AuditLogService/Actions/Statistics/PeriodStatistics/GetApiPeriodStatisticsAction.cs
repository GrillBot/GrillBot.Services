using AuditLogService.Core.Entity;
using GrillBot.Services.Common.Actions.Statistics;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics.PeriodStatistics;

public class GetApiPeriodStatisticsAction(IServiceProvider serviceProvider) : PeriodStatisticsActionBase<AuditLogServiceContext>(serviceProvider)
{
    protected override async Task<Dictionary<DateOnly, long>> GetRawDataAsync()
    {
        var apiGroups = GetParameter<string[]>(1);

        var query = DbContext.ApiRequests.AsNoTracking()
            .Where(o => apiGroups.Contains(o.ApiGroupName))
            .GroupBy(o => o.RequestDate)
            .Select(o => new
            {
                o.Key,
                Count = o.LongCount()
            });

        return await ContextHelper.ReadToDictionaryAsync(query, o => o.Key, o => o.Count);
    }
}
