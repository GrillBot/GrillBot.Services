using GrillBot.Core.Metrics.Collectors;
using GrillBot.Core.Metrics.Components;

namespace PointsService.Telemetry;

public class PointsTelemetryCollector : ITelemetryCollector
{
    public TelemetryGauge TransactionsToMerge { get; } = new("transactions_to_merge", null, "Count of expired transactions for merge.");
    public TelemetryGauge ActiveTransactions { get; } = new("active_transactions", null, "Count of active transactions.");

    public IEnumerable<TelemetryCollectorComponent> GetComponents()
    {
        yield return TransactionsToMerge;
        yield return ActiveTransactions;
    }
}
