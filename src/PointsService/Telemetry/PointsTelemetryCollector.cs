using GrillBot.Services.Common.Telemetry.Gauge;

namespace PointsService.Telemetry;

public class PointsTelemetryCollector
{
    public TelemetryGaugeCollector<int> TransactionsToMerge { get; } = new();
    public TelemetryGaugeCollector<int> ActiveTransactionsCount { get; } = new();
}
