using GrillBot.Core;
using GrillBot.Services.Common;
using Microsoft.EntityFrameworkCore;
using PointsService.Actions;
using PointsService.Core.Entity;
using PointsService.Core.Options;
using PointsService.Core.Providers;
using PointsService.Handlers;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    args,
    (services, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services.AddDatabaseContext<PointsServiceContext>(b => b.UseNpgsql(connectionString));
        services.AddStatisticsProvider<StatisticsProvider>();
        services.AddSwaggerGen();
        services.AddActions();
        services.AddRabbitMQ();
    },
    configureHealthChecks: (healthCheckBuilder, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;
        healthCheckBuilder.AddNpgSql(connectionString);
    },
    preRunInitialization: (app, _) => app.InitDatabaseAsync<PointsServiceContext>(),
    configureDevOnlyMiddleware: app =>
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
);

await application.RunAsync();
