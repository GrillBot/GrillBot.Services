using GrillBot.Core.Database;
using Microsoft.EntityFrameworkCore;
using SearchingService.Core.Entity;

namespace SearchingService.Core.Providers;

public class StatisticsProvider(SearchingServiceContext _dbContext) : IStatisticsProvider
{
    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
    {
        return new Dictionary<string, long>
        {
            { nameof(_dbContext.Items), await _dbContext.Items.CountAsync() },
            { nameof(_dbContext.Users), await _dbContext.Users.CountAsync() }
        };
    }
}
