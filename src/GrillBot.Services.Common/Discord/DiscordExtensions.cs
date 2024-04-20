using Discord;
using Discord.Rest;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GrillBot.Services.Common.Discord;

public static class DiscordExtensions
{
    public static void AddDiscord(this IServiceCollection services, IConfiguration configuration)
    {
        if (string.IsNullOrEmpty(configuration.GetConnectionString("BotToken")))
            return;

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

    public static async Task InitDiscordAsync(this IApplicationBuilder app, IServiceProvider scopedProvider)
    {
        app.ApplicationServices.GetService<DiscordLogManager>();

        var manager = scopedProvider.GetService<DiscordManager>();
        if (manager is not null)
            await manager.LoginAsync();
    }
}
