using System.Diagnostics.Metrics;

namespace GrillBot.Services.Common.Telemetry;

public class TelemetryCounterCollector(IEnumerable<KeyValuePair<string, object?>>? tags = null)
{
    private readonly object _locker = new();
    private long _value;

    public void Increment()
    {
        lock (_locker)
        {
            _value++;
        }
    }

    public Measurement<long> Get()
    {
        lock (_locker)
        {
            return new Measurement<long>(_value, tags ?? []);
        }
    }
}
