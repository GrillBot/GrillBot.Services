using GrillBot.Core;
using GrillBot.Services.Common;
using Microsoft.EntityFrameworkCore;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Core.Providers;
using UserMeasuresService.Options;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    args,
    (services, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services.AddDatabaseContext<UserMeasuresContext>(b => b.UseNpgsql(connectionString));
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
