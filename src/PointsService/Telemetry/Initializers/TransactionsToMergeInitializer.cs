using GrillBot.Core.Metrics.Initializer;
using PointsService.Actions.Merge;

namespace PointsService.Telemetry.Initializers;

public class TransactionsToMergeInitializer(
    IServiceProvider serviceProvider,
    PointsTelemetryCollector _collector
) : TelemetryInitializer(serviceProvider)
{
    protected override async Task ExecuteInternalAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var action = provider.GetRequiredService<MergeValidTransactionsAction>();
        _collector.TransactionsToMerge.Set(await action.CountAsync());
    }
}
