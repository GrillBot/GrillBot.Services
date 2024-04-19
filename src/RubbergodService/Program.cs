using GrillBot.Core;
using GrillBot.Services.Common;
using Microsoft.EntityFrameworkCore;
using RubbergodService.Core;
using RubbergodService.Core.Entity;
using RubbergodService.Core.Providers;
using RubbergodService.Core.Repository;
using RubbergodService.DirectApi;
using RubbergodService.Discord;
using System.Reflection;

var application = await ServiceBuilder.CreateWebAppAsync(
    Assembly.GetExecutingAssembly(),
    args,
    (services, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services.AddDatabaseContext<RubbergodServiceContext>(b => b.UseNpgsql(connectionString));
        services.AddScoped<RubbergodServiceRepository>();
        services.AddMemoryCache();
        services.AddStatisticsProvider<StatisticsProvider>();
        services.AddDiscord();
        services.AddDirectApi();
    },
    configureHealthChecks: (healthCheckBuilder, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;
        healthCheckBuilder.AddNpgSql(connectionString);
    },
    preRunInitialization: async (app, scopedProvider) =>
    {
        app.ApplicationServices.GetRequiredService<DiscordLogManager>();

        await app.InitDatabaseAsync<RubbergodServiceContext>();
        await scopedProvider.GetRequiredService<DiscordManager>().LoginAsync();
    }
);

await application.RunAsync();
