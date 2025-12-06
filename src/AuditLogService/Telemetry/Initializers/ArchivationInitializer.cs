using AuditLogService.Actions.Archivation;
using GrillBot.Services.Common.Telemetry;

namespace AuditLogService.Telemetry.Initializers;

public class ArchivationInitializer(
    IServiceProvider serviceProvider,
    AuditLogTelemetryCollector _collector
) : TelemetryInitializerBase(serviceProvider)
{
    protected override async Task ExecuteInternalAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var action = provider.GetRequiredService<CreateArchivationDataAction>();
        _collector.ItemsOfArchive.Set(await action.CountAsync());
    }
}
