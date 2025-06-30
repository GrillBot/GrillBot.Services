using GrillBot.Core.Metrics.Collectors;
using GrillBot.Core.Metrics.Components;

namespace AuditLogService.Telemetry;

public class AuditLogTelemetryCollector : ITelemetryCollector
{
    public TelemetryGaugeContainer CountOfFiles { get; } = new("count_of_files", "Count of files per type stored in the audit log.");
    public TelemetryGaugeContainer SizeOfFiles { get; } = new("size_of_files", "Size of files per type stored in the audit log");
    public TelemetryGauge ItemsOfArchive { get; } = new("items_to_archive", null, "Count of items pending to archivation.");
    public TelemetryGaugeContainer ApiAvgDurations { get; } = new("api_avg_durations", "AVG duration of REST endpoints in the bot.");
    public TelemetryGaugeContainer JobsAvgDurations { get; } = new("jobs_avg_durations", "AVG duration of scheduled jobs.");

    public void SetApiAvgDuration(string endpoint, int avgDuration)
        => ApiAvgDurations.Set(endpoint, avgDuration, new Dictionary<string, object?> { ["endpoint"] = endpoint });

    public void SetJobsAvgDuration(string name, int avgDuration)
        => JobsAvgDurations.Set(name, avgDuration, new Dictionary<string, object?> { ["job"] = name });

    public void SetFilesCount(string type, int count)
        => CountOfFiles.Set(type, count, new() { ["type"] = type });

    public void SetFileSizes(string type, long size)
        => SizeOfFiles.Set(type, size, new() { ["type"] = type });

    public IEnumerable<TelemetryCollectorComponent> GetComponents()
    {
        yield return CountOfFiles;
        yield return SizeOfFiles;
        yield return ItemsOfArchive;
        yield return ApiAvgDurations;
        yield return JobsAvgDurations;
    }
}
