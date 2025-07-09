using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace GrillBot.Services.Common.Actions.Statistics;

public abstract class PeriodStatisticsActionBase<TDbContext>(
    TDbContext dbContext,
    ICounterManager counterManager
) : ApiAction<TDbContext>(counterManager, dbContext)
    where TDbContext : DbContext
{
    protected abstract Task<Dictionary<DateOnly, long>> GetRawDataAsync();

    public override async Task<ApiResult> ProcessAsync()
    {
        var groupingKey = GetParameter<string>(0);
        var rawData = await GetRawDataAsync();
        var groupedData = GetGroupedData(rawData, groupingKey);

        return ApiResult.Ok(groupedData);
    }

    private static Dictionary<string, long> GetGroupedData(Dictionary<DateOnly, long> rawData, string groupingKey)
    {
        return groupingKey switch
        {
            "ByYear" => rawData
                .GroupBy(o => o.Key.Year)
                .OrderBy(o => o.Key)
                .ToDictionary(o => o.Key.ToString(), o => o.Sum(x => x.Value)),
            "ByMonth" => rawData
                .GroupBy(o => new { o.Key.Year, o.Key.Month })
                .OrderBy(o => o.Key.Year).ThenBy(o => o.Key.Month)
                .ToDictionary(o => $"{o.Key.Year}-{o.Key.Month.ToString().PadLeft(2, '0')}", o => o.Sum(x => x.Value)),
            "ByDate" => rawData
                .OrderBy(o => o.Key)
                .ToDictionary(o => o.Key.ToString("o"), o => o.Value),
            _ => throw new NotSupportedException()
        };
    }
}
