using GrillBot.Core.Metrics.Collectors;
using GrillBot.Core.Metrics.Components;

namespace AuditLogService.Telemetry;

public class AuditLogTelemetryCollector : ITelemetryCollector
{
    public TelemetryGauge CountOfFiles { get; } = new("count_of_files", null, "Count of files stored in the audit log.");
    public TelemetryGauge SizeOfFiles { get; } = new("size_of_files", null, "Size of files stored in the audit log");
    public TelemetryGauge ItemsOfArchive { get; } = new("items_to_archive", null, "Count of items pending to archivation.");
    public TelemetryGaugeContainer ApiAvgDurations { get; } = new("api_avg_durations", "AVG duration of REST endpoints in the bot.");
    public TelemetryGaugeContainer JobsAvgDurations { get; } = new("jobs_avg_durations", "AVG duration of scheduled jobs.");
    public TelemetryGaugeContainer ApiRequestCountsByDay { get; } = new("api_requests_count_by_day", "Count of API requests by day.");

    public void SetApiAvgDuration(string endpoint, int avgDuration)
        => ApiAvgDurations.Set(endpoint, avgDuration, new Dictionary<string, object?> { ["endpoint"] = endpoint });

    public void SetJobsAvgDuration(string name, int avgDuration)
        => JobsAvgDurations.Set(name, avgDuration, new Dictionary<string, object?> { ["job"] = name });

    public void SetApiRequestCountsByDay(string apiGroup, DateOnly requestDate, long count)
    {
        var tags = new Dictionary<string, object?>
        {
            ["group"] = apiGroup,
            ["date"] = requestDate.ToString("o")
        };

        ApiRequestCountsByDay.Set($"{apiGroup}/{requestDate}", count, tags);
    }

    public IEnumerable<TelemetryCollectorComponent> GetComponents()
    {
        yield return CountOfFiles;
        yield return SizeOfFiles;
        yield return ItemsOfArchive;
        yield return ApiAvgDurations;
        yield return JobsAvgDurations;
        yield return ApiRequestCountsByDay;
    }
}
