using Discord;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Cache;
using Microsoft.Extensions.Caching.Memory;

namespace AuditLogService.Cache;

public class GuildCache : InMemoryCache<IGuild>
{
    public GuildCache(IMemoryCache cache, ICounterManager counterManager) : base(counterManager, cache)
    {
    }

    public IGuild? GetGuild(ulong guildId) => TryReadData(guildId.ToString(), out var guild) ? guild : null;
    public void StoreGuild(IGuild guild) => Write(guild.Id.ToString(), guild, DateTimeOffset.Now.AddHours(4));
}
