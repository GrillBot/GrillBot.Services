using GrillBot.Services.Common.EntityFramework;

namespace GrillBot.Services.Common.Telemetry.Database.Initializers;

public class DefaultDatabaseInitializer<TDbContext>(
    IServiceProvider serviceProvider,
    DatabaseTelemetryCollector _collector
) : TelemetryInitializerBase(serviceProvider) where TDbContext : GrillBotServiceDbContext
{
    protected override async Task ExecuteInternalAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var contextHelper = CreateContextHelper<TDbContext>(provider);
        var tables = await contextHelper.DbContext.GetRecordsCountInTablesAsync();

        foreach (var (name, count) in tables)
            _collector.SetTableCount(name, count);
    }
}
