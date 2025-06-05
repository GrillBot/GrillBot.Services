using AuditLogService.Core.Entity;
using GrillBot.Services.Common.Telemetry;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Telemetry.Initializers;

public class FilesInitializer(
    IServiceProvider serviceProvider,
    AuditLogTelemetryCollector _collector
) : TelemetryInitializerBase(serviceProvider)
{
    protected override async Task ExecuteInternalAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var contextHelper = CreateContextHelper<AuditLogServiceContext>(provider);
        var query = contextHelper.DbContext.Files.AsNoTracking();

        _collector.CountOfFiles.Set(await contextHelper.ReadCountAsync(query));
        _collector.SizeOfFiles.Set(await contextHelper.ReadSumAsync(query, o => o.Size));
    }
}
