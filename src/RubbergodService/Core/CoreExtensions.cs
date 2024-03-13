using Discord;
using Discord.Rest;
using RubbergodService.Discord;

namespace RubbergodService.Core;

public static class CoreExtensions
{
    public static void AddDiscord(this IServiceCollection services)
    {
        var config = new DiscordRestConfig
        {
            LogLevel = LogSeverity.Verbose,
            FormatUsersInBidirectionalUnicode = false
        };

        services
            .AddSingleton<IDiscordClient>(new DiscordRestClient(config))
            .AddSingleton<DiscordLogManager>()
            .AddScoped<DiscordManager>();
    }
}
