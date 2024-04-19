using GrillBot.Core.Services;
using GrillBot.Services.Common;
using ImageProcessingService.Caching;
using ImageProcessingService.Core;
using ImageProcessingService.Core.Options;
using System.Reflection;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    Assembly.GetExecutingAssembly(),
    args,
    (services, configuration) =>
    {
        services.AddCaching();
        services.AddExternalServices(configuration);
    },
    configureKestrel: options => options.Limits.MaxRequestBodySize = 1073741824, // 1GB
    configureHealthChecks: (healthCheckBuilder, _) => healthCheckBuilder.AddCheck<GraphicsServiceHealthCheck>(nameof(GraphicsServiceHealthCheck))
);

await application.RunAsync();