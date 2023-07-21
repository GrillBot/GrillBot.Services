using AuditLogService.BackgroundServices.Actions;
using AuditLogService.Core.Entity;
using System.Threading.Channels;

namespace AuditLogService.BackgroundServices;

public static class PostProcessingExtensions
{
    public static void AddPostProcessing(this IServiceCollection services)
    {
        var channelOptions = new BoundedChannelOptions(int.MaxValue) { FullMode = BoundedChannelFullMode.Wait };
        var channel = Channel.CreateBounded<LogItem>(channelOptions);
        services.AddSingleton(channel);

        services.AddHostedService<PostProcessingService>();

        services
            .AddScoped<PostProcessActionBase, ComputeApiDateCountsAction>()
            .AddScoped<PostProcessActionBase, ComputeApiResultCountsAction>()
            .AddScoped<PostProcessActionBase, ComputeApiRequestStatsAction>()
            .AddScoped<PostProcessActionBase, ComputeAvgTimesAction>()
            .AddScoped<PostProcessActionBase, ComputeApiUserStatisticsAction>()
            .AddScoped<PostProcessActionBase, ComputeInteractionUserStatisticsAction>()
            .AddScoped<PostProcessActionBase, DeleteInvalidStatisticsAction>();
    }
}
