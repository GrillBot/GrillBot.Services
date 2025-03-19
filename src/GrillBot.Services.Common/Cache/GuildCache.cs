using Discord;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Cache.Abstraction;
using Microsoft.Extensions.Caching.Memory;

namespace GrillBot.Services.Common.Cache;

public class GuildCache(
    IMemoryCache memoryCache,
    ICounterManager counterManager
) : InMemoryCache<IGuild>(counterManager, memoryCache)
{
    public IGuild? GetGuild(ulong guildId) => TryReadData(guildId.ToString(), out var guild) ? guild : null;
    public void StoreGuild(IGuild guild) => Write(guild.Id.ToString(), guild, DateTimeOffset.Now.AddHours(8));
}
