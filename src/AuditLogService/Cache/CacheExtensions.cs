namespace AuditLogService.Cache;

public static class CacheExtensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<GuildCache>();
        services.AddScoped<AuditLogCache>();

        return services;
    }
}
