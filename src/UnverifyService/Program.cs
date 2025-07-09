using GrillBot.Core;
using GrillBot.Core.Metrics;
using GrillBot.Services.Common;
using GrillBot.Services.Common.EntityFramework.Extensions;
using GrillBot.Services.Common.Providers;
using GrillBot.Services.Common.Telemetry.Database.Initializers;
using System.Reflection;
using UnverifyService.Core.Entity;
using UnverifyService.Options;
using UnverifyService.Telemetry;
using UnverifyService.Telemetry.Initializers;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    Assembly.GetExecutingAssembly(),
    args,
    (services, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services.AddPostgresDatabaseContext<UnverifyContext>(connectionString);
        services.AddStatisticsProvider<DefaultStatisticsProvider<UnverifyContext>>();
        services.AddTelemetryInitializer<DefaultDatabaseInitializer<UnverifyContext>>();
        services.AddTelemetryCollector<UnverifyTelemetryCollector>();
        services.AddTelemetryInitializer<ArchivationInitializer>();
        services.AddTelemetryInitializer<ActiveUnverifyInitializer>();
    },
    configureHealthChecks: (healthCheckBuilder, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;
        healthCheckBuilder.AddNpgSql(connectionString);
    },
    preRunInitialization: (app, _) => app.InitDatabaseAsync<UnverifyContext>()
);

await application.RunAsync();
