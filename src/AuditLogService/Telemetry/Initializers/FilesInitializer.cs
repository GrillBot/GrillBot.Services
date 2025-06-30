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
        var query = contextHelper.DbContext.Files.AsNoTracking()
            .GroupBy(o => o.Extension ?? ".NoExtension")
            .Select(o => new
            {
                Key = o.Key.StartsWith('.') ? o.Key.Substring(1) : o.Key,
                Count = o.Count(),
                Size = o.Sum(x => x.Size)
            });

        var items = await query.ToListAsync();
        foreach (var item in items)
        {
            _collector.SetFilesCount(item.Key, item.Count);
            _collector.SetFileSizes(item.Key, item.Size);
        }
    }
}
