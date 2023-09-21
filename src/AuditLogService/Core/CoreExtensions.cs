using AuditLogService.Actions;
using AuditLogService.BackgroundServices;
using AuditLogService.Cache;
using AuditLogService.Core.Discord;
using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Options;
using AuditLogService.Core.Providers;
using AuditLogService.Processors;
using GrillBot.Core;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;

namespace AuditLogService.Core;

public static class CoreExtensions
{
    public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services
            .AddDatabaseContext<AuditLogServiceContext>(b => b.UseNpgsql(connectionString))
            .AddDatabaseContext<AuditLogStatisticsContext>(b => b.UseNpgsql(connectionString));

        services
            .AddDiagnostic()
            .AddCoreManagers()
            .AddStatisticsProvider<StatisticsProvider>()
            .AddControllers(c => c.RegisterCoreFilter());

        // HealthChecks
        services
            .AddHealthChecks()
            .AddNpgSql(connectionString);

        // OpenAPI
        services
            .AddEndpointsApiExplorer()
            .AddSwaggerGen();

        services.Configure<RouteOptions>(opt => opt.LowercaseUrls = true);
        services.Configure<ForwardedHeadersOptions>(opt => opt.ForwardedHeaders = ForwardedHeaders.All);
        services.Configure<AppOptions>(configuration);

        services.AddActions();
        services.AddProcessors();
        services.AddDiscord();
        services.AddPostProcessing();
        services.AddCaching();
    }
}
