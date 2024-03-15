using EmoteService.Core.Entity;
using GrillBot.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Core.Providers;

public class StatisticsProvider : IStatisticsProvider
{
    private readonly EmoteServiceContext _dbContext;

    public StatisticsProvider(EmoteServiceContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
    {
        return new Dictionary<string, long>
        {
            { nameof(_dbContext.EmoteDefinitions), await _dbContext.EmoteDefinitions.LongCountAsync() },
            { nameof(_dbContext.EmoteUserStatItems), await _dbContext.EmoteUserStatItems.LongCountAsync() }
        };
    }
}
