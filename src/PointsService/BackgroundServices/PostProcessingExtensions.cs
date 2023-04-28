using PointsService.BackgroundServices.PostProcessAction;

namespace PointsService.BackgroundServices;

public static class PostProcessingExtensions
{
    public static IServiceCollection AddPostProcessing(this IServiceCollection services)
    {
        services
            .AddScoped<PostProcessActionBase, RecalculatePositionAction>()
            .AddScoped<PostProcessActionBase, RecalculateLeaderboardAction>()
            .AddScoped<PostProcessActionBase, CalculateDailyStatsAction>();

        return services
            .AddSingleton<PostProcessingQueue>()
            .AddHostedService<PostProcessingService>();
    }
}
