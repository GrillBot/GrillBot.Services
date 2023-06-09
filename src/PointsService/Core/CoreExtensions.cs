﻿using GrillBot.Core;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using PointsService.Actions;
using PointsService.BackgroundServices;
using PointsService.Core.Entity;
using PointsService.Core.Options;
using PointsService.Core.Providers;
using PointsService.Core.Repository;
using PointsService.Validation;

namespace PointsService.Core;

public static class CoreExtensions
{
    public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")!;

        services
            .AddDatabaseContext<PointsServiceContext>(builder => builder.UseNpgsql(connectionString))
            .AddScoped<PointsServiceRepository>();

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
        services.AddValidations();
        services.AddPostProcessing();
    }
}
