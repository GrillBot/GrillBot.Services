using GrillBot.Core.Database;

namespace UserMeasuresService.Core.Providers;

public class StatisticsProvider : IStatisticsProvider
{
    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
    {
        return new Dictionary<string, long>();
    }
}
