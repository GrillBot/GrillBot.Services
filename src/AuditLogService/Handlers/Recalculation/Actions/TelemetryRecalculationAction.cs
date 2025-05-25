using AuditLogService.Models.Events.Recalculation;
using AuditLogService.Telemetry;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Handlers.Recalculation.Actions;

public class TelemetryRecalculationAction(IServiceProvider serviceProvider) : RecalculationActionBase(serviceProvider)
{
    private readonly AuditLogTelemetryCollector _telemetryCollector
        = serviceProvider.GetRequiredService<AuditLogTelemetryCollector>();

    public override bool CheckPreconditions(RecalculationPayload payload)
        => payload.FilesCount > 0;

    public override async Task ProcessAsync(RecalculationPayload payload)
    {
        if (payload.FilesCount > 0)
            await RecalculateFilesAsync();
    }

    private async Task RecalculateFilesAsync()
    {
        var baseQuery = DbContext.Files.AsNoTracking();

        _telemetryCollector.CountOfFiles.Set(await baseQuery.CountAsync());
        _telemetryCollector.SizeOfFiles.Set(await baseQuery.SumAsync(o => o.Size));
    }
}
