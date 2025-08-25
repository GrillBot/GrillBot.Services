using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Options;
using AuditLogService.Core.Providers;
using AuditLogService.Managers;
using AuditLogService.Processors;
using AuditLogService.Telemetry;
using AuditLogService.Telemetry.Initializers;
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

        services
            .AddPostgresDatabaseContext<AuditLogServiceContext>(connectionString)
            .AddPostgresDatabaseContext<AuditLogStatisticsContext>(connectionString);

        services.AddStatisticsProvider<StatisticsProvider>();
        services.AddManagers();

        services.AddScoped<RequestProcessorFactory>();

        services.AddTelemetryCollector<AuditLogTelemetryCollector>();
        services.AddTelemetryInitializer<FilesInitializer>();
        services.AddTelemetryInitializer<DefaultDatabaseInitializer<AuditLogServiceContext>>();
        services.AddTelemetryInitializer<DefaultDatabaseInitializer<AuditLogStatisticsContext>>();
        services.AddTelemetryInitializer<ApiStatisticsInitializer>();
        services.AddTelemetryInitializer<JobStatisticsInitializer>();
        services.AddTelemetryInitializer<ArchivationInitializer>();
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