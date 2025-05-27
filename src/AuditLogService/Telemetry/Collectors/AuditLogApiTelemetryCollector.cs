using GrillBot.Services.Common.Telemetry;
using System.Diagnostics.Metrics;

namespace AuditLogService.Telemetry.Collectors;

public class AuditLogApiTelemetryCollector
{
    private readonly Dictionary<string, TelemetryGaugeCollector<int>> _apiEndpoints = [];
    private readonly object _lock = new();

    public void Set(string endpoint, int avgDuration)
    {
        lock (_lock)
        {
            if (!_apiEndpoints.TryGetValue(endpoint, out TelemetryGaugeCollector<int>? value))
            {
                value = new([KeyValuePair.Create<string, object?>("endpoint", endpoint)]);
                _apiEndpoints.Add(endpoint, value);
            }

            value.Set(avgDuration);
        }
    }

    public IEnumerable<Measurement<int>> GetMeasurements()
    {
        lock (_lock)
        {
            return [.. _apiEndpoints.Select(o => o.Value.Get())];
        }
    }
}
