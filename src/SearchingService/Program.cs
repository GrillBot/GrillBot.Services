using GrillBot.Core;
using GrillBot.Core.Metrics;
using GrillBot.Services.Common;
using GrillBot.Services.Common.Telemetry.Database.Initializers;
using Microsoft.EntityFrameworkCore;
using SearchingService.Core.Entity;
using SearchingService.Core.Providers;
using SearchingService.Options;
using System.Reflection;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    Assembly.GetExecutingAssembly(),
    args,
    (services, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services.AddDbContext<SearchingServiceContext>(opt => opt.UseNpgsql(connectionString));
        services.AddStatisticsProvider<StatisticsProvider>();
        services.AddTelemetryInitializer<DefaultDatabaseInitializer<SearchingServiceContext>>();
    },
    configureHealthChecks: (healthCheckBuilder, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;
        healthCheckBuilder.AddNpgSql(connectionString);
    },
    preRunInitialization: (app, _) => app.InitDatabaseAsync<SearchingServiceContext>()
);

await application.RunAsync();