using RubbergodService.Actions.Pins;

namespace RubbergodService.Actions;

public static class ActionExtensions
{
    public static IServiceCollection AddActions(this IServiceCollection services)
    {
        // Pins
        services
            .AddScoped<GetPinsAction>()
            .AddScoped<InvalidateCacheAction>();
        
        return services
            .AddScoped<SendDirectApiAction>()
            .AddScoped<StoreKarmaAction>()
            .AddScoped<GetKarmaPageAction>();
    }
}
