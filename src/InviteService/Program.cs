using GrillBot.Core;
using GrillBot.Services.Common;
using InviteService.Core.Entity;
using InviteService.Core.Providers;
using InviteService.Options;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    Assembly.GetExecutingAssembly(),
    args,
    (services, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDatabaseContext<InviteContext>(b => b.UseNpgsql(connectionString));
        services.AddStatisticsProvider<StatisticsProvider>();
    },
    configureHealthChecks: (healthCheckBuilder, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;
        healthCheckBuilder.AddNpgSql(connectionString);
    },
    preRunInitialization: (app, _) => app.InitDatabaseAsync<InviteContext>()
);

await application.RunAsync();