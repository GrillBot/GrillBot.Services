using EmoteService.Core.Entity;
using EmoteService.Core.Options;
using EmoteService.Core.Providers;
using EmoteService.Telemetry.Initializers;
using GrillBot.Core;
using GrillBot.Core.Metrics;
using GrillBot.Services.Common;
using GrillBot.Services.Common.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    Assembly.GetExecutingAssembly(),
    args,
    (services, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services.AddPostgresDatabaseContext<EmoteServiceContext>(connectionString);
        services.AddStatisticsProvider<StatisticsProvider>();
        services.AddTelemetryInitializer<DatabaseInitializer>();
    },
    configureHealthChecks: (builder, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;
        builder.AddNpgSql(connectionString);
    },
    preRunInitialization: (app, _) => app.InitDatabaseAsync<EmoteServiceContext>()
);

await application.RunAsync();
