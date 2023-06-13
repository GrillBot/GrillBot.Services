using Discord;
using GrillBot.Core.Managers.Performance;

namespace AuditLogService.Core.Discord.Cache;

public class GuildCache : GuildCacheBase<IGuild>
{
    public GuildCache(ICounterManager counterManager) : base(counterManager)
    {
    }

    public IGuild? GetGuild(ulong guildId)
    {
        using (CounterManager.Create("Discord.Cache.Guild"))
        {
            lock (Locker)
            {
                return CachedData.TryGetValue(guildId, out var guild) ? guild : null;
            }
        }
    }

    public void StoreGuild(ulong guildId, IGuild guild)
    {
        using (CounterManager.Create("Discord.Cache.Guild"))
        {
            lock (Locker)
            {
                CachedData[guildId] = guild;
            }
        }
    }

    protected override void DisposeItem(IGuild data)
    {
    }
}
