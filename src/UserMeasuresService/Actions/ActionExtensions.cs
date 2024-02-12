using UserMeasuresService.Actions.Dashboard;

namespace UserMeasuresService.Actions;

public static class ActionExtensions
{
    public static IServiceCollection AddActions(this IServiceCollection services)
    {
        // Dashboard
        services.AddScoped<GetDashboardData>();

        return services;
    }
}
