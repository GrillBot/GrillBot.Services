using System.Diagnostics.Metrics;

namespace GrillBot.Services.Common.Telemetry.Gauge;

public class TelemetryGaugeCollectorContainer<TValue>(
    IEnumerable<KeyValuePair<string, object?>>? _tags = null
) where TValue : struct
{
    private readonly Dictionary<string, TelemetryGaugeCollector<TValue>> _collectors = [];
    private readonly object _lock = new();

    public void Set(string key, TValue value)
    {
        lock (_lock)
        {
            if (!_collectors.TryGetValue(key, out var gauge))
            {
                gauge = CreateGauge(key);
                _collectors.Add(key, gauge);
            }

            gauge.Set(value);
        }
    }

    public IEnumerable<Measurement<TValue>> GetMeasurements()
    {
        lock (_lock)
        {
            return [.. _collectors.Values.Select(o => o.Get())];
        }
    }

    protected virtual TelemetryGaugeCollector<TValue> CreateGauge(string key)
        => new(_tags);
}
