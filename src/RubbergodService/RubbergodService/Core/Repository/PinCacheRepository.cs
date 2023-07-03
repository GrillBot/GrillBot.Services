using GrillBot.Core.Database.Repository;
using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using RubbergodService.Core.Entity;

namespace RubbergodService.Core.Repository;

public class PinCacheRepository : SubRepositoryBase<RubbergodServiceContext>
{
    public PinCacheRepository(RubbergodServiceContext context, ICounterManager counterManager) : base(context, counterManager)
    {
    }

    public async Task<List<PinCacheItem>> FindItemsByChannelAsync(ulong guildId, ulong channelId)
    {
        using (CreateCounter())
        {
            return await Context.PinCache
                .Where(o => o.GuildId == guildId.ToString() && o.ChannelId == channelId.ToString())
                .ToListAsync();
        }
    }
}
