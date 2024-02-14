using GrillBot.Core.Database;
using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;

namespace PointsService.Core.Providers;

public class StatisticsProvider : IStatisticsProvider
{
    private PointsServiceContext DbContext { get; }
    private ICounterManager CounterManager { get; }

    public StatisticsProvider(PointsServiceContext dbContext, ICounterManager counterManager)
    {
        DbContext = dbContext;
        CounterManager = counterManager;
    }

    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
    {
        using (CounterManager.Create("Database.Statistics"))
        {
            return new Dictionary<string, long>
            {
                { nameof(DbContext.Channels), await DbContext.Channels.LongCountAsync() },
                { nameof(DbContext.Users), await DbContext.Users.LongCountAsync() },
                { nameof(DbContext.Transactions), await DbContext.Transactions.LongCountAsync() },
                { nameof(DbContext.Leaderboard), await DbContext.Leaderboard.LongCountAsync() },
                { nameof(DbContext.DailyStats), await DbContext.DailyStats.LongCountAsync() }
            };
        }
    }
}
