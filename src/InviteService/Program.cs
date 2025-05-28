using GrillBot.Core;
using GrillBot.Core.Metrics;
using GrillBot.Core.Redis;
using GrillBot.Services.Common;
using GrillBot.Services.Common.EntityFramework.Extensions;
using GrillBot.Services.Common.Telemetry.Database.Initializers;
using InviteService.Core.Entity;
using InviteService.Core.Providers;
using InviteService.Options;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    Assembly.GetExecutingAssembly(),
    args,
    (services, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services.AddPostgresDatabaseContext<InviteContext>(connectionString);
        services.AddStatisticsProvider<StatisticsProvider>();
        services.AddRedisDistributedCache(configuration);
        services.AddTelemetryInitializer<DefaultDatabaseInitializer<InviteContext>>();
    },
    configureHealthChecks: (healthCheckBuilder, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;
        healthCheckBuilder.AddNpgSql(connectionString);
    },
    preRunInitialization: (app, _) => app.InitDatabaseAsync<InviteContext>()
);

await application.RunAsync();