using GrillBot.Core.Database.Repository;
using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;

namespace PointsService.Core.Repository;

public class LeaderboardRepository : SubRepositoryBase<PointsServiceContext>
{
    public LeaderboardRepository(PointsServiceContext context, ICounterManager counterManager) : base(context, counterManager)
    {
    }

    private IQueryable<LeaderboardItem> GetBaseQuery(string guildId, bool disableTracking)
    {
        var query = Context.Leaderboard.Where(o => o.GuildId == guildId);
        if (disableTracking)
            query = query.AsNoTracking();
        return query;
    }

    public async Task<int> ComputeLeaderboardCountAsync(string guildId)
    {
        using (CreateCounter())
        {
            return await GetBaseQuery(guildId, true).CountAsync();
        }
    }
}
