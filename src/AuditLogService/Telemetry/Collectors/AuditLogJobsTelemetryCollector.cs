using GrillBot.Services.Common.Telemetry.Gauge;

namespace AuditLogService.Telemetry.Collectors;

public class AuditLogJobsTelemetryCollector : TelemetryGaugeCollectorContainer<int>
{
    protected override TelemetryGaugeCollector<int> CreateGauge(string key)
    {
        return new([KeyValuePair.Create<string, object?>("job", key)]);
    }
}
