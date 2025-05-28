using AuditLogService.Core.Enums;
using AuditLogService.Models.Events.Recalculation;
using AuditLogService.Telemetry.Collectors;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Handlers.Recalculation.Actions;

public class TelemetryRecalculationAction(IServiceProvider serviceProvider) : RecalculationActionBase(serviceProvider)
{
    private readonly AuditLogTelemetryCollector _telemetryCollector
        = serviceProvider.GetRequiredService<AuditLogTelemetryCollector>();

    private readonly AuditLogApiTelemetryCollector _apiTelemetryCollector
        = serviceProvider.GetRequiredService<AuditLogApiTelemetryCollector>();

    private readonly AuditLogJobsTelemetryCollector _jobsTelemetryCollector
        = serviceProvider.GetRequiredService<AuditLogJobsTelemetryCollector>();

    public override bool CheckPreconditions(RecalculationPayload payload)
    {
        return payload.FilesCount > 0 ||
            (payload.Type is LogType.Api && payload.Api is not null) ||
            (payload.Type is LogType.JobCompleted && payload.Job is not null);
    }

    public override async Task ProcessAsync(RecalculationPayload payload)
    {
        if (payload.FilesCount > 0)
            await RecalculateFilesAsync();

        if (payload.Type is LogType.Api && payload.Api is not null)
            await RecalculateApiAsync(payload);

        if (payload.Type is LogType.JobCompleted && payload.Job is not null)
            await RecalculateJobsAsync(payload);
    }

    private async Task RecalculateFilesAsync()
    {
        var baseQuery = DbContext.Files.AsNoTracking();

        _telemetryCollector.CountOfFiles.Set(await baseQuery.CountAsync());
        _telemetryCollector.SizeOfFiles.Set(await baseQuery.SumAsync(o => o.Size));
    }

    private async Task RecalculateApiAsync(RecalculationPayload payload)
    {
        var endpoint = $"{payload.Api!.Method} {payload.Api.TemplatePath}";
        var query = StatisticsContext.RequestStats.AsNoTracking()
            .Where(o => o.Endpoint == endpoint)
            .Select(o => (int?)Math.Round(o.TotalDuration / (double)(o.SuccessCount + o.FailedCount)));

        var data = await query.FirstOrDefaultAsync();
        if (data is not null)
            _apiTelemetryCollector.Set(endpoint, data.Value);
    }

    private async Task RecalculateJobsAsync(RecalculationPayload payload)
    {
        var query = StatisticsContext.JobInfos.AsNoTracking()
            .Where(o => o.Name == payload.Job!.JobName)
            .Select(o => (int?)Math.Round(o.TotalDuration / (double)o.StartCount));

        var data = await query.FirstOrDefaultAsync();
        if (data is not null)
            _jobsTelemetryCollector.Set(payload.Job!.JobName, data.Value);
    }
}
