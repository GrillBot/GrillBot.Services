using GrillBot.Core;
using GrillBot.Core.Services;
using ImageProcessingService.Actions;
using ImageProcessingService.Caching;
using ImageProcessingService.Core.Options;
using Microsoft.AspNetCore.HttpOverrides;

namespace ImageProcessingService.Core;

public static class CoreExtensions
{
    public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddDiagnostic()
            .AddCoreManagers()
            .AddCacheServices()
            .AddControllers(c => c.RegisterCoreFilter());

        // HealthChecks
        services
            .AddHealthChecks()
            .AddCheck<GraphicsServiceHealthCheck>(nameof(GraphicsServiceHealthCheck));

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

        // Graphics service
        services.AddExternalServices(configuration);
    }
}
