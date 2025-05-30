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
        var baseQuery = DbContext.Files.AsNoTracking();

        _telemetryCollector.CountOfFiles.Set(await baseQuery.CountAsync());
        _telemetryCollector.SizeOfFiles.Set(await baseQuery.SumAsync(o => o.Size));
    }
}
