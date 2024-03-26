namespace ImageProcessingService.Caching;

public static class CacheExtensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddMemoryCache();

        return services
            .AddScoped<PeepoCache>()
            .AddScoped<PointsCache>()
            .AddScoped<WithoutAccidentCache>();
    }
}
