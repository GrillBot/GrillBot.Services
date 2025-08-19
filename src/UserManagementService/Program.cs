using GrillBot.Core;
using GrillBot.Core.Metrics;
using GrillBot.Core.Services;
using GrillBot.Services.Common;
using GrillBot.Services.Common.EntityFramework.Extensions;
using GrillBot.Services.Common.Providers;
using GrillBot.Services.Common.Telemetry.Database.Initializers;
using System.Reflection;
using UserManagementService.Core.Entity;
using UserManagementService.Options;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    Assembly.GetExecutingAssembly(),
    args,
    (services, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services.AddPostgresDatabaseContext<UserManagementContext>(connectionString);
        services.AddStatisticsProvider<DefaultStatisticsProvider<UserManagementContext>>();
        services.AddTelemetryInitializer<DefaultDatabaseInitializer<UserManagementContext>>();
        services.AddExternalServices(configuration);
    },
    configureHealthChecks: (healthCheckBuilder, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;
        healthCheckBuilder.AddNpgSql(connectionString);
    },
    preRunInitialization: (app, _) => app.InitDatabaseAsync<UserManagementContext>()
);

await application.RunAsync();