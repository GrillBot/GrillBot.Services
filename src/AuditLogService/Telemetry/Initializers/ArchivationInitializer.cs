using AuditLogService.Actions.Archivation;
using GrillBot.Core.Metrics.Initializer;

namespace AuditLogService.Telemetry.Initializers;

public class ArchivationInitializer(
    IServiceProvider serviceProvider,
    AuditLogTelemetryCollector _collector
) : TelemetryInitializer(serviceProvider)
{
    protected override async Task ExecuteInternalAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var action = provider.GetRequiredService<CreateArchivationDataAction>();
        _collector.ItemsOfArchive.Set(await action.CountAsync());
    }
}
