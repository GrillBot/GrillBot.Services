using GrillBot.Core.Metrics.Collectors;
using GrillBot.Core.Metrics.Components;

namespace UnverifyService.Telemetry;

public class UnverifyTelemetryCollector : ITelemetryCollector
{
    public TelemetryGauge LogsToArchive { get; } = new("logs_to_archive", null, "Count of logs to archive.");
    public TelemetryGauge ActiveUnverify { get; } = new("active_unverify", null, "Count of active unverifies.");
    public TelemetryGauge ActiveSelfUnverify { get; } = new("active_self_unverify", null, "Count of active unverifies.");

    public IEnumerable<TelemetryCollectorComponent> GetComponents()
    {
        yield return LogsToArchive;
        yield return ActiveUnverify;
        yield return ActiveSelfUnverify;
    }
}
