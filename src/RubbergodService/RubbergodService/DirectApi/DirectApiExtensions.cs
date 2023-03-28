namespace RubbergodService.DirectApi;

public static class DirectApiExtensions
{
    public static IServiceCollection AddDirectApi(this IServiceCollection services)
    {
        return services
            .AddSingleton<DirectApiClient>();
    }
}
