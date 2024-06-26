using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Options;
using AuditLogService.Core.Providers;
using AuditLogService.Managers;
using AuditLogService.Processors;
using GrillBot.Core;
using GrillBot.Services.Common;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    Assembly.GetExecutingAssembly(),
    args,
    (services, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services
            .AddDatabaseContext<AuditLogServiceContext>(b => b.UseNpgsql(connectionString))
            .AddDatabaseContext<AuditLogStatisticsContext>(b => b.UseNpgsql(connectionString));

        services.AddStatisticsProvider<StatisticsProvider>();
        services.AddManagers();

        services.AddScoped<RequestProcessorFactory>();
    },
    configureHealthChecks: (builder, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;
        builder.AddNpgSql(connectionString);
    },
    preRunInitialization: async (app, _) =>
    {
        await app.InitDatabaseAsync<AuditLogServiceContext>();
        await app.InitDatabaseAsync<AuditLogStatisticsContext>();
    }
);

await application.RunAsync();