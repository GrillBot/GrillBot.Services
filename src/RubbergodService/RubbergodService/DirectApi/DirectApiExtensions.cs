namespace RubbergodService.DirectApi;

public static class DirectApiExtensions
{
    public static void AddDirectApi(this IServiceCollection services)
    {
        services
            .AddSingleton<DirectApiClient>()
            .AddScoped<DirectApiManager>();
    }
}
