using GrillBot.Core.Database;

namespace EmoteService.Core.Providers;

public class StatisticsProvider : IStatisticsProvider
{
    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
    {
        return new Dictionary<string, long>();
    }
}
