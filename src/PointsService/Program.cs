using GrillBot.Core;
using GrillBot.Services.Common;
using GrillBot.Services.Common.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;
using PointsService.Core.Options;
using PointsService.Core.Providers;
using System.Reflection;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    Assembly.GetExecutingAssembly(),
    args,
    (services, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services.AddPostgresDatabaseContext<PointsServiceContext>(connectionString);
        services.AddStatisticsProvider<StatisticsProvider>();
    },
    configureHealthChecks: (healthCheckBuilder, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;
        healthCheckBuilder.AddNpgSql(connectionString);
    },
    preRunInitialization: (app, _) => app.InitDatabaseAsync<PointsServiceContext>()
);

await application.RunAsync();
