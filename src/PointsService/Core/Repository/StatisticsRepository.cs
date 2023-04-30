using GrillBot.Core.Database.Repository;
using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;

namespace PointsService.Core.Repository;

public class StatisticsRepository : RepositoryBase<PointsServiceContext>
{
    public StatisticsRepository(PointsServiceContext context, ICounterManager counterManager) : base(context, counterManager)
    {
    }

    public async Task<Dictionary<string, long>> GetStatisticsAsync()
    {
        return new Dictionary<string, long>
        {
            { nameof(Context.Channels), await Context.Channels.LongCountAsync() },
            { nameof(Context.Users), await Context.Users.LongCountAsync() },
            { nameof(Context.Transactions), await Context.Transactions.LongCountAsync() },
            { nameof(Context.Leaderboard), await Context.Leaderboard.LongCountAsync() },
            { nameof(Context.DailyStats), await Context.DailyStats.LongCountAsync() }
        };
    }
}
