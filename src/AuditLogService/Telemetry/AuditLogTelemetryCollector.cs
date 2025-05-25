using GrillBot.Services.Common.Telemetry;

namespace AuditLogService.Telemetry;

public class AuditLogTelemetryCollector
{
    public TelemetryGaugeCollector<int> CountOfFiles { get; } = new();
    public TelemetryGaugeCollector<long> SizeOfFiles { get; } = new();
}
