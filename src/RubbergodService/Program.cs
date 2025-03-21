using GrillBot.Core;
using GrillBot.Core.Redis;
using GrillBot.Services.Common;
using Microsoft.EntityFrameworkCore;
using RubbergodService.Core.Entity;
using RubbergodService.Core.Providers;
using RubbergodService.DirectApi;
using System.Reflection;

var application = await ServiceBuilder.CreateWebAppAsync(
    Assembly.GetExecutingAssembly(),
    args,
    (services, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services.AddDatabaseContext<RubbergodServiceContext>(b => b.UseNpgsql(connectionString));
        services.AddRedis(configuration);
        services.AddStatisticsProvider<StatisticsProvider>();
        services.AddDirectApi();
        services.AddHttpClient();
    },
    configureHealthChecks: (healthCheckBuilder, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;
        healthCheckBuilder.AddNpgSql(connectionString);
    },
    preRunInitialization: async (app, _) => await app.InitDatabaseAsync<RubbergodServiceContext>()
);

await application.RunAsync();
