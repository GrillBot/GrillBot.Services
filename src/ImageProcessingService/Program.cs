using GrillBot.Core.Services;
using GrillBot.Services.Common;
using ImageProcessingService.Actions;
using ImageProcessingService.Caching;
using ImageProcessingService.Core;
using ImageProcessingService.Core.Options;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    args,
    (services, configuration) =>
    {
        services.AddCaching();
        services.AddSwaggerGen();
        services.AddActions();
        services.AddExternalServices(configuration);
    },
    configureKestrel: options => options.Limits.MaxRequestBodySize = 1073741824, // 1GB
    configureHealthChecks: (healthCheckBuilder, _) => healthCheckBuilder.AddCheck<GraphicsServiceHealthCheck>(nameof(GraphicsServiceHealthCheck)),
    configureDevOnlyMiddleware: app =>
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
);

await application.RunAsync();