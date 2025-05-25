using System.Collections.Concurrent;
using System.Diagnostics.Metrics;

namespace GrillBot.Services.Common.Telemetry.Database;

public class DatabaseTelemetryCollector
{
    private readonly Dictionary<string, TelemetryGaugeCollector<int>> _databaseTables = [];
    private readonly object _lock = new();

    public void Set(string entity, int records)
    {
        lock (_lock)
        {
            if (!_databaseTables.ContainsKey(entity))
            {
                _databaseTables.TryAdd(
                    entity,
                    new([KeyValuePair.Create<string, object?>("table", entity)])
                );
            }

            _databaseTables[entity].Set(records);
        }
    }

    public IEnumerable<Measurement<int>> GetMeasurements()
    {
        lock (_lock)
        {
            return [.. _databaseTables.Select(o => o.Value.Get())];
        }
    }
}
