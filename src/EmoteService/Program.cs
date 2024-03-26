using EmoteService.Core.Entity;
using EmoteService.Core.Options;
using EmoteService.Core.Providers;
using GrillBot.Core;
using GrillBot.Services.Common;
using Microsoft.EntityFrameworkCore;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    args,
    (services, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services.AddDatabaseContext<EmoteServiceContext>(b => b.UseNpgsql(connectionString));
        services.AddStatisticsProvider<StatisticsProvider>();
    },
    configureHealthChecks: (builder, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;
        builder.AddNpgSql(connectionString);
    },
    preRunInitialization: (app, _) => app.InitDatabaseAsync<EmoteServiceContext>()
);

await application.RunAsync();
