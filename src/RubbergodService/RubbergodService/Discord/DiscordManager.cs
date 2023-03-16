using Discord;
using Discord.Rest;
using GrillBot.Core.Managers.Performance;
using Microsoft.Extensions.Caching.Memory;

namespace RubbergodService.Discord;

public class DiscordManager
{
    private IDiscordClient Client { get; }
    private IConfiguration Configuration { get; }
    private IMemoryCache MemoryCache { get; }
    private ICounterManager CounterManager { get; }

    public DiscordManager(IDiscordClient client, IConfiguration configuration, IMemoryCache memoryCache, ICounterManager counterManager)
    {
        Client = client;
        Configuration = configuration;
        MemoryCache = memoryCache;
        CounterManager = counterManager;
    }

    public async Task LoginAsync()
    {
        var token = Configuration.GetConnectionString("DiscordBot") ?? throw new ArgumentNullException(nameof(Configuration));

        using (CounterManager.Create("Discord.API.Login"))
        {
            await ((DiscordRestClient)Client).LoginAsync(TokenType.Bot, token);
        }
    }

    public async Task<IUser?> GetUserAsync(ulong id)
    {
        var cacheKey = $"User_{id}";
        IUser? user;

        using (CounterManager.Create("Discord.Cache"))
        {
            if (MemoryCache.TryGetValue(cacheKey, out user))
                return user;
        }

        using (CounterManager.Create("Discord.API.GetUser"))
        {
            user = await Client.GetUserAsync(id);
        }

        if (user is null)
            return null;

        using (CounterManager.Create("Discord.Cache"))
        {
            MemoryCache.Set(cacheKey, user, DateTimeOffset.Now.AddSeconds(30));
        }

        return user;
    }
}
