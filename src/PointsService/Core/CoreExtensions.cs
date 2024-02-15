using GrillBot.Core;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using PointsService.Actions;
using PointsService.Core.Entity;
using PointsService.Core.Options;
using PointsService.Core.Providers;
using PointsService.Handlers;

namespace PointsService.Core;

public static class CoreExtensions
{
    public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services
            .AddDatabaseContext<PointsServiceContext>(builder => builder.UseNpgsql(connectionString));

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

        // Configuration
        services.Configure<RouteOptions>(opt => opt.LowercaseUrls = true);
        services.Configure<ForwardedHeadersOptions>(opt => opt.ForwardedHeaders = ForwardedHeaders.All);
        services.Configure<AppOptions>(configuration);

        // Actions
        services.AddActions();
        services.AddRabbitMQ();
    }
}
