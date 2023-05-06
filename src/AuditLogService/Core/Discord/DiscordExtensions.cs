using Discord;
using Discord.Rest;

namespace AuditLogService.Core.Discord;

public static class DiscordExtensions
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
