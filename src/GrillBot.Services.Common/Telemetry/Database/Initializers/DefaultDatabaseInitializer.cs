using GrillBot.Core.Metrics.Initializer;
using GrillBot.Services.Common.EntityFramework;
using Microsoft.Extensions.DependencyInjection;

namespace GrillBot.Services.Common.Telemetry.Database.Initializers;

public class DefaultDatabaseInitializer<TDbContext>(
    IServiceProvider serviceProvider,
    DatabaseTelemetryCollector _collector
) : TelemetryInitializer(serviceProvider) where TDbContext : GrillBotServiceDbContext
{
    protected override async Task ExecuteInternalAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var context = provider.GetRequiredService<TDbContext>();
        var tables = await context.GetRecordsCountInTablesAsync();

        foreach (var (name, count) in tables)
            _collector.SetTableCount(name, count);
    }
}
