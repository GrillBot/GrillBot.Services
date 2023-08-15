using RubbergodService.Actions.Help;
using RubbergodService.Actions.Pins;

namespace RubbergodService.Actions;

public static class ActionExtensions
{
    public static IServiceCollection AddActions(this IServiceCollection services)
    {

        // Help
        services
            .AddScoped<GetSlashCommandsAction>();

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
