using GrillBot.Core;
using GrillBot.Core.Metrics;
using GrillBot.Services.Common;
using GrillBot.Services.Common.EntityFramework.Extensions;
using GrillBot.Services.Common.Telemetry.Database.Initializers;
using System.Reflection;
using UserManagementService.Core.Entity;
using UserManagementService.Core.Providers;
using UserManagementService.Options;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    Assembly.GetExecutingAssembly(),
    args,
    (services, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services.AddPostgresDatabaseContext<UserManagementContext>(connectionString);
        services.AddStatisticsProvider<StatisticsProvider>();
        services.AddTelemetryInitializer<DefaultDatabaseInitializer<UserManagementContext>>();
    },
    configureHealthChecks: (healthCheckBuilder, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;
        healthCheckBuilder.AddNpgSql(connectionString);
    },
    preRunInitialization: (app, _) => app.InitDatabaseAsync<UserManagementContext>()
);

await application.RunAsync();