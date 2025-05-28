using EmoteService.Core.Entity;
using EmoteService.Core.Options;
using EmoteService.Core.Providers;
using GrillBot.Core;
using GrillBot.Core.Metrics;
using GrillBot.Services.Common;
using GrillBot.Services.Common.EntityFramework.Extensions;
using GrillBot.Services.Common.Telemetry.Database.Initializers;
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
        services.AddTelemetryInitializer<DefaultDatabaseInitializer<EmoteServiceContext>>();
    },
    configureHealthChecks: (builder, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;
        builder.AddNpgSql(connectionString);
    },
    preRunInitialization: (app, _) => app.InitDatabaseAsync<EmoteServiceContext>()
);

await application.RunAsync();
