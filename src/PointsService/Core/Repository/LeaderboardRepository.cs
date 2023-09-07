using GrillBot.Core.Database.Repository;
using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;
using PointsService.Models;

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

    public async Task<LeaderboardItem?> FindItemAsync(string guildId, string userId, bool disableTracking = false)
    {
        using (CreateCounter())
        {
            return await GetBaseQuery(guildId, disableTracking)
                .FirstOrDefaultAsync(o => o.UserId == userId);
        }
    }

    public async Task<int> ComputeLeaderboardCountAsync(string guildId)
    {
        using (CreateCounter())
        {
            return await GetBaseQuery(guildId, true).CountAsync();
        }
    }

    public async Task<int> ComputePositionAsync(string guildId, string userId)
    {
        using (CreateCounter())
        {
            var data = await GetBaseQuery(guildId, true)
                .OrderByDescending(o => o.YearBack)
                .Select(o => o.UserId)
                .ToListAsync();
            
            return data.FindIndex(o => o == userId) + 1;
        }
    }

    public async Task<int> ComputeMaxPositionAsync(string guildId)
    {
        using (CreateCounter())
        {
            return await GetBaseQuery(guildId, true).CountAsync();
        }
    }

    public async Task<bool> HaveSomePointsAsync(string guildId, string userId)
    {
        using (CreateCounter())
        {
            return await GetBaseQuery(guildId, true)
                .AnyAsync(o => o.UserId == userId && o.YearBack > 0);
        }
    }
}
