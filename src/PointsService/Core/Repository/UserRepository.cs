using GrillBot.Core.Database.Repository;
using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;

namespace PointsService.Core.Repository;

public class UserRepository : RepositoryBase<PointsServiceContext>
{
    public UserRepository(PointsServiceContext context, ICounterManager counterManager) : base(context, counterManager)
    {
    }

    private IQueryable<User> GetQueryBase(string guildId, string userId, bool disableTracking)
    {
        var query = Context.Users
            .Where(o => o.GuildId == guildId && o.Id == userId);
        if (disableTracking)
            query = query.AsNoTracking();

        return query;
    }

    public bool ExistsUser(string guildId, string userId)
    {
        using (CreateCounter())
        {
            return GetQueryBase(guildId, userId, true).Any();
        }
    }

    public async Task<User?> FindUserAsync(string guildId, string userId, bool disableTracking = false)
    {
        using (CreateCounter())
        {
            return await GetQueryBase(guildId, userId, disableTracking).FirstOrDefaultAsync();
        }
    }

    public async Task<User?> FindFirstUserForPostProcessing()
    {
        using (CreateCounter())
        {
            return await Context.Users.AsNoTracking()
                .FirstOrDefaultAsync(o => o.PendingRecalculation);
        }
    }

    public async Task<List<User>> FindUsersWithSamePositionAsync(string guildId, string exceptUserId, int position)
    {
        using (CreateCounter())
        {
            return await Context.Users
                .Where(o => o.GuildId == guildId && o.Id != exceptUserId && o.PointsPosition == position)
                .ToListAsync();
        }
    }
}
