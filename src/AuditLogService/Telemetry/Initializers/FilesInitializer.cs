using AuditLogService.Core.Entity;
using GrillBot.Core.Metrics.Initializer;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Telemetry.Initializers;

public class FilesInitializer(
    IServiceProvider serviceProvider,
    AuditLogTelemetryCollector _collector
) : TelemetryInitializer(serviceProvider)
{
    protected override async Task ExecuteInternalAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var context = provider.GetRequiredService<AuditLogServiceContext>();
        var query = context.Files.AsNoTracking();

        _collector.CountOfFiles.Set(await query.CountAsync(cancellationToken));
        _collector.SizeOfFiles.Set(await query.SumAsync(o => o.Size, cancellationToken));
    }
}
