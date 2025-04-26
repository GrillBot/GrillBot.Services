using GrillBot.Core;
using GrillBot.Services.Common;
using GrillBot.Services.Common.EntityFramework.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Core.Providers;
using UserMeasuresService.Options;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    Assembly.GetExecutingAssembly(),
    args,
    (services, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services.AddPostgresDatabaseContext<UserMeasuresContext>(connectionString);
        services.AddStatisticsProvider<StatisticsProvider>();
    },
    configureHealthChecks: (healthCheckBuilder, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;
        healthCheckBuilder.AddNpgSql(connectionString);
    },
    preRunInitialization: (app, _) => app.InitDatabaseAsync<UserMeasuresContext>()
);

await application.RunAsync();
