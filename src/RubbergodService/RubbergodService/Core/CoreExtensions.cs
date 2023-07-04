using Discord;
using Discord.Rest;
using GrillBot.Core;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using RubbergodService.Actions;
using RubbergodService.Core.Entity;
using RubbergodService.Core.Providers;
using RubbergodService.Core.Repository;
using RubbergodService.DirectApi;
using RubbergodService.Discord;

namespace RubbergodService.Core;

public static class CoreExtensions
{
    public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services
            .AddDatabaseContext<RubbergodServiceContext>(builder => builder.UseNpgsql(connectionString))
            .AddScoped<RubbergodServiceRepository>();

        services
            .AddMemoryCache()
            .AddDiagnostic()
            .AddCoreManagers()
            .AddStatisticsProvider<StatisticsProvider>()
            .AddControllers(c => c.RegisterCoreFilter());

        // HealthChecks
        services
            .AddHealthChecks()
            .AddNpgSql(connectionString);

        // OpenAPI
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();

        // Discord
        services.AddDiscord();

        // Configuration
        services.Configure<RouteOptions>(opt => opt.LowercaseUrls = true);
        services.Configure<ForwardedHeadersOptions>(opt => opt.ForwardedHeaders = ForwardedHeaders.All);

        // Actions
        services.AddActions();
        services.AddDirectApi();
    }

    private static void AddDiscord(this IServiceCollection services)
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
