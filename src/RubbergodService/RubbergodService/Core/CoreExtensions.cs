using Discord;
using Discord.Rest;
using GrillBot.Core;
using Microsoft.EntityFrameworkCore;
using RubbergodService.Core.Entity;
using RubbergodService.Core.Repository;
using RubbergodService.DirectApi;
using RubbergodService.Discord;
using RubbergodService.Managers;
using RubbergodService.MemberSynchronization;

namespace RubbergodService.Core;

public static class CoreExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration, out string connectionString)
    {
        var connString = configuration.GetConnectionString("Default")!;
        connectionString = connString;

        return services
            .AddDatabaseContext<RubbergodServiceContext>(b => b.UseNpgsql(connString))
            .AddScoped<RubbergodServiceRepository>();
    }

    public static IServiceCollection AddManagers(this IServiceCollection services)
    {
        return services
            .AddScoped<KarmaManager>()
            .AddScoped<UserManager>();
    }

    public static IServiceCollection AddDiscord(this IServiceCollection services)
    {
        var config = new DiscordRestConfig
        {
            LogLevel = LogSeverity.Verbose,
            FormatUsersInBidirectionalUnicode = false
        };

        var client = new DiscordRestClient(config);
        services
            .AddSingleton<IDiscordClient>(client)
            .AddSingleton<DiscordLogManager>()
            .AddScoped<DiscordManager>();
        return services;
    }

    public static IServiceCollection AddDirectApi(this IServiceCollection services)
    {
        services
            .AddSingleton<DirectApiClient>()
            .AddScoped<DirectApiManager>();
        return services;
    }

    public static IServiceCollection AddMemberSync(this IServiceCollection services)
    {
        services
            .AddSingleton<MemberSyncQueue>()
            .AddHostedService<MemberSyncService>();
        return services;
    }
}
