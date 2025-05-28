using GrillBot.Core.Metrics;
using GrillBot.Services.Common.Telemetry.Database;
using Microsoft.Extensions.Hosting;

namespace GrillBot.Services.Common.Telemetry;

public static class ServiceCollectionExtensions
{
    public static void AddTelemetryFeatures(this IHostApplicationBuilder builder)
    {
        builder.AddTelemetry();
        builder.Services.AddTelemetryCollector<DatabaseTelemetryCollector>();
    }
}
