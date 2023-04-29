namespace ImageProcessingService.Caching;

public static class CacheExtensions
{
    public static IServiceCollection AddCacheServices(this IServiceCollection services)
    {
        services.AddMemoryCache();

        services
            .AddScoped<PeepoCache>()
            .AddScoped<PointsCache>();
        return services;
    }
}
