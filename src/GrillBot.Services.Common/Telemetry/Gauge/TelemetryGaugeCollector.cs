using System.Diagnostics.Metrics;

namespace GrillBot.Services.Common.Telemetry.Gauge;

public class TelemetryGaugeCollector<TValue>(IEnumerable<KeyValuePair<string, object?>>? tags = null) where TValue : struct
{
    private readonly object _locker = new();
    private TValue _value;

    public void Set(TValue value)
    {
        lock (_locker)
        {
            _value = value;
        }
    }

    public Measurement<TValue> Get()
    {
        lock (_locker)
        {
            return new Measurement<TValue>(_value, tags ?? []);
        }
    }
}
