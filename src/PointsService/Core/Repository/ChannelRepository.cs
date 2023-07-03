using GrillBot.Core.Database.Repository;
using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;

namespace PointsService.Core.Repository;

public class ChannelRepository : SubRepositoryBase<PointsServiceContext>
{
    public ChannelRepository(PointsServiceContext context, ICounterManager counterManager) : base(context, counterManager)
    {
    }

    private IQueryable<Channel> GetQueryBase(string guildId, string channelId, bool disableTracking)
    {
        var query = Context.Channels.Where(o => o.GuildId == guildId && o.Id == channelId);
        if (disableTracking)
            query = query.AsNoTracking();
        return query;
    }

    public bool ExistsChannel(string guildId, string channelId)
    {
        using (CreateCounter())
        {
            return GetQueryBase(guildId, channelId, true).Any();
        }
    }

    public async Task<Channel?> FindChannelAsync(string guildId, string channelId, bool disableTracking = false)
    {
        using (CreateCounter())
        {
            return await GetQueryBase(guildId, channelId, disableTracking).FirstOrDefaultAsync();
        }
    }
}
