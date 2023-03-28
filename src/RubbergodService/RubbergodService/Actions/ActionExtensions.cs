namespace RubbergodService.Actions;

public static class ActionExtensions
{
    public static IServiceCollection AddActions(this IServiceCollection services)
    {
        return services
            .AddScoped<SendDirectApiAction>()
            .AddScoped<StoreKarmaAction>()
            .AddScoped<GetKarmaPageAction>()
            .AddScoped<RefreshUserAction>();
    }
}
