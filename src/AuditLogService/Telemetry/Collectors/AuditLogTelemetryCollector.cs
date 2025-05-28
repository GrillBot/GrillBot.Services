using GrillBot.Services.Common.Telemetry.Gauge;

namespace AuditLogService.Telemetry.Collectors;

public class AuditLogTelemetryCollector
{
    public TelemetryGaugeCollector<int> CountOfFiles { get; } = new();
    public TelemetryGaugeCollector<long> SizeOfFiles { get; } = new();
    public TelemetryGaugeCollector<int> ItemsToArchive { get; } = new();
}
