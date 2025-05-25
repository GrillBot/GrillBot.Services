using GrillBot.Core.Metrics.CustomTelemetry;
using System.Diagnostics.Metrics;

namespace AuditLogService.Telemetry;

public class AuditLogTelemetryBuilder(AuditLogTelemetryCollector _telemetryCollector) : ICustomTelemetryBuilder
{
    public void BuildCustomTelemetry(Meter meter)
    {
        meter.CreateObservableGauge("count_of_files", _telemetryCollector.CountOfFiles.Get, description: "Count of files stored in the audit log");
        meter.CreateObservableGauge("size_of_files", _telemetryCollector.SizeOfFiles.Get, description: "Size of files stored in the audit log");
    }
}
