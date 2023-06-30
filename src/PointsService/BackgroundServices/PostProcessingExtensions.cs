using PointsService.BackgroundServices.Actions;

namespace PointsService.BackgroundServices;

public static class PostProcessingExtensions
{
    public static void AddPostProcessing(this IServiceCollection services)
    {
        services
            .AddScoped<PostProcessActionBase, RecalculateLeaderboardAction>()
            .AddScoped<PostProcessActionBase, RecalculatePositionAction>()
            .AddScoped<PostProcessActionBase, CalculateDailyStatsAction>();

        services
            .AddHostedService<PostProcessingService>();
    }
}
