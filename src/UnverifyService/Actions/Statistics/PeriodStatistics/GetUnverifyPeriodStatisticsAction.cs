using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Actions.Statistics;
using Microsoft.EntityFrameworkCore;
using UnverifyService.Core.Entity;
using UnverifyService.Core.Enums;

namespace UnverifyService.Actions.Statistics.PeriodStatistics;

public class GetUnverifyPeriodStatisticsAction(
    UnverifyContext dbContext,
    ICounterManager counterManager
) : PeriodStatisticsActionBase<UnverifyContext>(dbContext, counterManager)
{
    protected override async Task<Dictionary<DateOnly, long>> GetRawDataAsync()
    {
        var operationType = GetParameter<UnverifyOperationType>(1);

        var query = DbContext.LogItems.AsNoTracking()
            .Where(o => o.OperationType == operationType)
            .GroupBy(o => o.CreatedAt.Date)
            .Select(o => new
            {
                Key = DateOnly.FromDateTime(o.Key),
                Count = o.LongCount()
            });

        return await ContextHelper.ReadToDictionaryAsync(query, o => o.Key, o => o.Count, CancellationToken);
    }
}
