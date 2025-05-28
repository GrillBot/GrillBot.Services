using GrillBot.Core.Metrics.CustomTelemetry;
using System.Diagnostics.Metrics;

namespace PointsService.Telemetry;

public class PointsTelemetryBuilder(PointsTelemetryCollector _collector) : ICustomTelemetryBuilder
{
    public void BuildCustomTelemetry(Meter meter)
    {
        meter.CreateObservableGauge("transactions_to_merge", _collector.TransactionsToMerge.Get, description: "Count of expired transactions for merge.");
        meter.CreateObservableGauge("active_transactions", _collector.ActiveTransactionsCount.Get, description: "Count of active transactions.");
    }
}
