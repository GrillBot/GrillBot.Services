using GrillBot.Core.Redis;

namespace ImageProcessingService.Caching;

public static class CacheExtensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRedisDistributedCache(configuration);

        return services
            .AddScoped<PeepoCache>()
            .AddScoped<PointsCache>()
            .AddScoped<WithoutAccidentCache>();
    }
}
