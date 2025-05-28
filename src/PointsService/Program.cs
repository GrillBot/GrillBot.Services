using GrillBot.Core;
using GrillBot.Core.Metrics;
using GrillBot.Services.Common;
using GrillBot.Services.Common.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;
using PointsService.Core.Options;
using PointsService.Core.Providers;
using PointsService.Telemetry;
using System.Reflection;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    Assembly.GetExecutingAssembly(),
    args,
    (services, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services.AddPostgresDatabaseContext<PointsServiceContext>(connectionString);
        services.AddStatisticsProvider<StatisticsProvider>();

        services.AddHostedService<PointsTelemetryInitService>();
        services.AddSingleton<PointsTelemetryCollector>();
        services.AddCustomTelemetryBuilder<PointsTelemetryBuilder>();
    },
    configureHealthChecks: (healthCheckBuilder, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;
        healthCheckBuilder.AddNpgSql(connectionString);
    },
    preRunInitialization: (app, _) => app.InitDatabaseAsync<PointsServiceContext>()
);

await application.RunAsync();
