using UserMeasuresService.Actions.Dashboard;
using UserMeasuresService.Actions.Info;
using UserMeasuresService.Actions.MeasuresList;
using UserMeasuresService.Actions.User;

namespace UserMeasuresService.Actions;

public static class ActionExtensions
{
    public static IServiceCollection AddActions(this IServiceCollection services)
    {
        // Dashboard
        services.AddScoped<GetDashboardData>();

        // Info
        services.AddScoped<GetItemsCountOfGuild>();

        // User
        services.AddScoped<GetUserInfo>();

        // List
        services.AddScoped<GetMeasuresList>();

        return services;
    }
}
