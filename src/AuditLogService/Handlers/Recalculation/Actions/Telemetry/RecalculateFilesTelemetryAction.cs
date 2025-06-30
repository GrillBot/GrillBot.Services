using AuditLogService.Models.Events.Recalculation;
using AuditLogService.Telemetry;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Handlers.Recalculation.Actions.Telemetry;

public class RecalculateFilesTelemetryAction(IServiceProvider serviceProvider) : RecalculationActionBase(serviceProvider)
{
    private readonly AuditLogTelemetryCollector _telemetryCollector
        = serviceProvider.GetRequiredService<AuditLogTelemetryCollector>();

    public override async Task ProcessAsync(RecalculationPayload payload)
    {
        var query = DbContext.Files.AsNoTracking()
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
            _telemetryCollector.SetFilesCount(item.Key, item.Count);
            _telemetryCollector.SetFileSizes(item.Key, item.Size);
        }
    }
}
