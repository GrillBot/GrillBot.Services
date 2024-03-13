using AuditLogService.Actions;
using AuditLogService.Cache;
using AuditLogService.Core.Discord;
using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Options;
using AuditLogService.Core.Providers;
using AuditLogService.Handlers;
using AuditLogService.Managers;
using AuditLogService.Processors;
using GrillBot.Core;
using GrillBot.Services.Common;
using Microsoft.EntityFrameworkCore;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    args,
    (services, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services
            .AddDatabaseContext<AuditLogServiceContext>(b => b.UseNpgsql(connectionString))
            .AddDatabaseContext<AuditLogStatisticsContext>(b => b.UseNpgsql(connectionString));

        services.AddStatisticsProvider<StatisticsProvider>();
        services.AddSwaggerGen();
        services.AddActions();
        services.AddDiscord();
        services.AddCaching();
        services.AddRabbitMQ();
        services.AddManagers();

        services.AddScoped<RequestProcessorFactory>();
    },
    configureHealthChecks: (builder, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;
        builder.AddNpgSql(connectionString);
    },
    preRunInitialization: async (app, scopedProvider) =>
    {
        app.ApplicationServices.GetRequiredService<DiscordLogManager>();

        await app.InitDatabaseAsync<AuditLogServiceContext>();
        await app.InitDatabaseAsync<AuditLogStatisticsContext>();
        await scopedProvider.GetRequiredService<DiscordManager>().LoginAsync();
    },
    configureDevOnlyMiddleware: app =>
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
);

await application.RunAsync();