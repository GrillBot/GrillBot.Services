using GrillBot.Core;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using UserMeasuresService.Actions;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Core.Providers;
using UserMeasuresService.Handlers;
using UserMeasuresService.Options;

namespace UserMeasuresService.Core;

public static class CoreExtensions
{
    public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services
            .AddDatabaseContext<UserMeasuresContext>(b => b.UseNpgsql(connectionString));

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
        services.AddRabbitMQHandlers();
    }
}
