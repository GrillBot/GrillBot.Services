using Discord;
using Discord.Rest;
using GrillBot.Core.Managers.Performance;

namespace RubbergodService.Discord;

public class DiscordManager
{
    private IDiscordClient Client { get; }
    private IConfiguration Configuration { get; }
    private ICounterManager CounterManager { get; }

    public DiscordManager(IDiscordClient client, IConfiguration configuration, ICounterManager counterManager)
    {
        Client = client;
        Configuration = configuration;
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
}
