using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Options;
using AuditLogService.Core.Providers;
using AuditLogService.Managers;
using AuditLogService.Processors;
using AuditLogService.Telemetry;
using AuditLogService.Telemetry.Collectors;
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

        services
            .AddPostgresDatabaseContext<AuditLogServiceContext>(connectionString)
            .AddPostgresDatabaseContext<AuditLogStatisticsContext>(connectionString);

        services.AddStatisticsProvider<StatisticsProvider>();
        services.AddManagers();

        services.AddScoped<RequestProcessorFactory>();

        services.AddHostedService<AuditLogTelemetryInitService>();
        services.AddSingleton<AuditLogTelemetryCollector>();
        services.AddSingleton<AuditLogApiTelemetryCollector>();
        services.AddCustomTelemetryBuilder<AuditLogTelemetryBuilder>();
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