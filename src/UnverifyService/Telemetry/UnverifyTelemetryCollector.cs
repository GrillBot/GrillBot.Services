using GrillBot.Core.Metrics.Collectors;
using GrillBot.Core.Metrics.Components;

namespace UnverifyService.Telemetry;

public class UnverifyTelemetryCollector : ITelemetryCollector
{
    public TelemetryGauge ItemsToArchive { get; } = new("items_to_archive", null, "Count of items pending to archivation.");
    public TelemetryGauge ActiveUnverify { get; } = new("active_unverify", null, "Count of active unverifies.");
    public TelemetryGauge ActiveSelfUnverify { get; } = new("active_self_unverify", null, "Count of active unverifies.");

    public IEnumerable<TelemetryCollectorComponent> GetComponents()
    {
        yield return ItemsToArchive;
        yield return ActiveUnverify;
        yield return ActiveSelfUnverify;
    }
}
