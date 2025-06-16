using GrillBot.Core.Database;
using GrillBot.Services.Common.EntityFramework;

namespace GrillBot.Services.Common.Providers;

public class DefaultStatisticsProvider<TDbContext>(TDbContext _dbContext) : IStatisticsProvider where TDbContext : GrillBotServiceDbContext
{
    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
    {
        var result = await _dbContext.GetRecordsCountInTablesAsync();
        return result.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);
    }
}
