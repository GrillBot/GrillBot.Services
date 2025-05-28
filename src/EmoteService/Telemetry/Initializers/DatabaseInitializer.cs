using EmoteService.Core.Entity;
using GrillBot.Core.Metrics.Initializer;
using GrillBot.Services.Common.Telemetry.Database;

namespace EmoteService.Telemetry.Initializers;

public class DatabaseInitializer(
    IServiceProvider serviceProvider,
    DatabaseTelemetryCollector _collector
) : TelemetryInitializer(serviceProvider)
{
    protected override async Task ExecuteInternalAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var context = provider.GetRequiredService<EmoteServiceContext>();
        var tables = await context.GetRecordsCountInTablesAsync();

        foreach (var (name, count) in tables)
            _collector.SetTableCount(name, count);
    }
}
