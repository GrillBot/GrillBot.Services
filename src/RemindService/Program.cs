using GrillBot.Core;
using GrillBot.Services.Common;
using Microsoft.EntityFrameworkCore;
using RemindService.Core.Entity;
using RemindService.Core.Providers;
using RemindService.Options;
using System.Reflection;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    Assembly.GetExecutingAssembly(),
    args,
    (services, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services.AddDbContext<RemindServiceContext>(opt => opt.UseNpgsql(connectionString));
        services.AddStatisticsProvider<StatisticsProvider>();
    },
    configureHealthChecks: (healthCheckBuilder, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;
        healthCheckBuilder.AddNpgSql(connectionString);
    },
    preRunInitialization: (app, _) => app.InitDatabaseAsync<RemindServiceContext>()
);

await application.RunAsync();