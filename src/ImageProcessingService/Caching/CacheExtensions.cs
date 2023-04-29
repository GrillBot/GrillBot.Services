using ImageProcessingService.Caching.Models;

namespace ImageProcessingService.Caching;

public static class CacheExtensions
{
    public static IServiceCollection AddCacheServices(this IServiceCollection services)
    {
        services.AddMemoryCache();

        return services
            .AddScoped<PeepoCache>()
            .AddScoped<PointsCache>()
            .AddScoped<WithoutAccidentCache>()
            .AddScoped<ChartCache>();
    }
}
