using GrillBot.Core.Database;
using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;

namespace PointsService.Core.Providers;

public class StatisticsProvider(PointsServiceContext _dbContext, ICounterManager _counterManager) : IStatisticsProvider
{
    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
    {
        using (_counterManager.Create("Database.Statistics"))
        {
            return new Dictionary<string, long>
            {
                { nameof(_dbContext.Channels), await _dbContext.Channels.LongCountAsync() },
                { nameof(_dbContext.Users), await _dbContext.Users.LongCountAsync() },
                { nameof(_dbContext.Transactions), await _dbContext.Transactions.LongCountAsync() },
                { nameof(_dbContext.Leaderboard), await _dbContext.Leaderboard.LongCountAsync() },
                { nameof(_dbContext.DailyStats), await _dbContext.DailyStats.LongCountAsync() }
            };
        }
    }
}
