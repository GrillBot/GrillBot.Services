using GrillBot.Core.Metrics.CustomTelemetry;
using System.Diagnostics.Metrics;

namespace GrillBot.Services.Common.Telemetry.Database;

public class DatabaseTelemetryBuilder(DatabaseTelemetryCollector _collector) : ICustomTelemetryBuilder
{
    public void BuildCustomTelemetry(Meter meter)
    {
        meter.CreateObservableGauge("database_table_count", _collector.GetMeasurements, description: "Count of records in the database.");
    }
}
