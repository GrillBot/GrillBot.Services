using GrillBot.Core.Database;
using Microsoft.EntityFrameworkCore;
using SearchingService.Core.Entity;

namespace SearchingService.Core.Providers;

public class StatisticsProvider : IStatisticsProvider
{
    private readonly SearchingServiceContext _dbContext;

    public StatisticsProvider(SearchingServiceContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
    {
        return new Dictionary<string, long>
        {
            { nameof(_dbContext.Items), await _dbContext.Items.CountAsync() },
            { nameof(_dbContext.Users), await _dbContext.Users.CountAsync() }
        };
    }
}
