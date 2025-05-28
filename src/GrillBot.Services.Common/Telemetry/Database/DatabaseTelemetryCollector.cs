using GrillBot.Services.Common.Telemetry.Gauge;

namespace GrillBot.Services.Common.Telemetry.Database;

public class DatabaseTelemetryCollector : TelemetryGaugeCollectorContainer<int>
{
    protected override TelemetryGaugeCollector<int> CreateGauge(string key)
    {
        return new([KeyValuePair.Create<string, object?>("table", key)]);
    }
}
