using AuditLogService.Models.Events.Recalculation;
using AuditLogService.Telemetry;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Handlers.Recalculation.Actions.Telemetry;

public class RecalculateJobsTelemetryAction(IServiceProvider serviceProvider) : RecalculationActionBase(serviceProvider)
{
    private readonly AuditLogTelemetryCollector _telemetryCollector
        = serviceProvider.GetRequiredService<AuditLogTelemetryCollector>();

    public override bool CheckPreconditions(RecalculationPayload payload)
        => payload.Job is not null;

    public override async Task ProcessAsync(RecalculationPayload payload)
    {
        var query = StatisticsContext.JobInfos.AsNoTracking()
            .Where(o => o.Name == payload.Job!.JobName)
            .Select(o => (int?)Math.Round(o.TotalDuration / (double)o.StartCount));

        var data = await query.FirstOrDefaultAsync();
        if (data is not null)
            _telemetryCollector.SetJobsAvgDuration(payload.Job!.JobName, data.Value);
    }
}
