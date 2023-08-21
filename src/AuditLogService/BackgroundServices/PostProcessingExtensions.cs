using AuditLogService.BackgroundServices.Actions;
using AuditLogService.Core.Entity;
using System.Threading.Channels;

namespace AuditLogService.BackgroundServices;

public static class PostProcessingExtensions
{
    public static void AddPostProcessing(this IServiceCollection services)
    {
        services.AddHostedService<PostProcessingService>();

        services
            .AddScoped<PostProcessActionBase, ComputeApiDateCountsAction>()
            .AddScoped<PostProcessActionBase, ComputeApiResultCountsAction>()
            .AddScoped<PostProcessActionBase, ComputeApiRequestStatsAction>()
            .AddScoped<PostProcessActionBase, ComputeApiUserStatisticsAction>()
            .AddScoped<PostProcessActionBase, ComputeInteractionUserStatisticsAction>()
            .AddScoped<PostProcessActionBase, ComputeInteractionStatisticsAction>()
            .AddScoped<PostProcessActionBase, ComputeAvgTimesAction>()
            .AddScoped<PostProcessActionBase, ComputeTypeStatitistics>()
            .AddScoped<PostProcessActionBase, ComputeDateStatisticsAction>()
            .AddScoped<PostProcessActionBase, ComputeFileExtensionStatisticsAction>()
            .AddScoped<PostProcessActionBase, ComputeJobInfoAction>();

        services
            .AddScoped<PostProcessActionBase, DeleteInvalidStatisticsAction>()
            .AddScoped<PostProcessActionBase, HardDeleteAction>();
    }
}
