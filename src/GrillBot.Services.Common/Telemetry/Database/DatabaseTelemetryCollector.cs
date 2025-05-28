using GrillBot.Core.Metrics.Collectors;
using GrillBot.Core.Metrics.Components;

namespace GrillBot.Services.Common.Telemetry.Database;

public class DatabaseTelemetryCollector : ITelemetryCollector
{
    public TelemetryGaugeContainer TableCounts { get; } = new("database_table_count", "Count of records in the database.");

    public void SetTableCount(string table, long count)
        => TableCounts.Set(table, count, new Dictionary<string, object?> { ["table"] = table });

    public IEnumerable<TelemetryCollectorComponent> GetComponents()
    {
        yield return TableCounts;
    }
}
