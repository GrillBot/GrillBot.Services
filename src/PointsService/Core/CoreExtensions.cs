using GrillBot.Core;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;
using PointsService.Core.Providers;
using PointsService.Core.Repository;

namespace PointsService.Core;

public static class CoreExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        // Database
        services.AddDbContext<PointsServiceContext>(builder => builder
            .UseNpgsql(connectionString)
            .EnableDetailedErrors()
            .EnableThreadSafetyChecks()
        ).AddScoped<PointsServiceRepository>();

        services
            .AddDiagnostic()
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

        // Forwarding
        services.Configure<ForwardedHeadersOptions>(opt => opt.ForwardedHeaders = ForwardedHeaders.All);

        return services;
    }
}
