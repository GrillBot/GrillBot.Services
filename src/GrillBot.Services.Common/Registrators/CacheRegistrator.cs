using GrillBot.Services.Common.Cache;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace GrillBot.Services.Common.Registrators;

public static class CacheRegistrator
{
    private static readonly Type _cacheType = typeof(CacheBase);

    private static readonly Type _inMemoryInterface = typeof(IInMemoryCache);
    private static readonly Type _scopedInterface = typeof(IScopedCache);

    public static void RegisterCacheFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var types = assembly.GetTypes();
        var caches = types.Where(o => o.IsClass && !o.IsAbstract && _cacheType.IsAssignableFrom(o)).ToList();

        if (caches.Count > 0)
            services.AddMemoryCache();

        foreach (var cache in caches)
        {
            if (cache.GetInterface(_inMemoryInterface.Name) is not null)
                services.AddSingleton(cache);
            else if (cache.GetInterface(_scopedInterface.Name) is not null)
                services.AddScoped(cache);
        }
    }
}
