using GrillBot.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GrillBot.Services.Common;

public static class ServiceBuilder
{
    public static async Task<WebApplication> CreateWebAppAsync<TAppOptions>(
        string[] args,
        Action<IServiceCollection, IConfiguration> configureServices,
        Action<KestrelServerOptions>? configureKestrel = null,
        Action<MvcOptions>? configureControllers = null,
        Action<IHealthChecksBuilder, IConfiguration>? configureHealthChecks = null,
        Func<IApplicationBuilder, IServiceProvider, Task>? preRunInitialization = null,
        Action<IApplicationBuilder>? configureMiddleware = null,
        Action<IApplicationBuilder>? configureDevOnlyMiddleware = null
    ) where TAppOptions : class
    {
        var builder = WebApplication.CreateBuilder(args);

        // Kestrel
        builder.WebHost.ConfigureKestrel(opt =>
        {
            opt.AddServerHeader = false;

            if (configureKestrel is not null)
                configureKestrel(opt);
        });

        // GrillBot.Core
        builder.Services.AddDiagnostic();
        builder.Services.AddCoreManagers();

        // MVC
        builder.Services.AddControllers(opt =>
        {
            opt.RegisterCoreFilter();

            if (configureControllers is not null)
                configureControllers(opt);
        });

        // HealthChecks
        var healthChecks = builder.Services.AddHealthChecks();
        if (configureHealthChecks is not null)
            configureHealthChecks(healthChecks, builder.Configuration);

        // OpenAPI
        builder.Services.AddEndpointsApiExplorer();

        // Static configuration
        builder.Services.Configure<RouteOptions>(opt => opt.LowercaseUrls = true);
        builder.Services.Configure<ForwardedHeadersOptions>(opt => opt.ForwardedHeaders = ForwardedHeaders.All);

        if (typeof(TAppOptions) != typeof(object))
            builder.Services.Configure<TAppOptions>(builder.Configuration);

        // Other services and configurations.
        configureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        if (preRunInitialization is not null)
        {
            using var scope = app.Services.CreateScope();
            await preRunInitialization(app, scope.ServiceProvider);
        }

        if (app.Environment.IsDevelopment() && configureDevOnlyMiddleware is not null)
            configureDevOnlyMiddleware(app);

        if (configureMiddleware is not null)
            configureMiddleware(app);

        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/health");

        return app;
    }

    public static Task<WebApplication> CreateWebAppAsync(
        string[] args,
        Action<IServiceCollection, IConfiguration> configureServices,
        Action<KestrelServerOptions>? configureKestrel = null,
        Action<MvcOptions>? configureControllers = null,
        Action<IHealthChecksBuilder, IConfiguration>? configureHealthChecks = null,
        Func<IApplicationBuilder, IServiceProvider, Task>? preRunInitialization = null,
        Action<IApplicationBuilder>? configureMiddleware = null,
        Action<IApplicationBuilder>? configureDevOnlyMiddleware = null
    ) => CreateWebAppAsync<object>(args, configureServices, configureKestrel, configureControllers, configureHealthChecks, preRunInitialization, configureMiddleware, configureDevOnlyMiddleware);
}
