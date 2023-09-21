using AuditLogService.Cache.Abstraction;
using Discord;
using GrillBot.Core.Managers.Performance;
using Microsoft.Extensions.Caching.Memory;

namespace AuditLogService.Cache;

public class GuildCache : InMemoryPersistentCache<IGuild>
{
    public GuildCache(IMemoryCache cache, ICounterManager counterManager) : base(cache, counterManager)
    {
    }

    public IGuild? GetGuild(ulong guildId) => Read(guildId.ToString());

    public void StoreGuild(IGuild guild)
        => Write(guild.Id.ToString(), guild, DateTimeOffset.Now.AddHours(4));
}
