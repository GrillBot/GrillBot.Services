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

    public override bool CheckPreconditions(RecalculationPayload payload)
        => payload.FilesCount > 0 || (payload.Type is LogType.Api && payload.Api is not null);

    public override async Task ProcessAsync(RecalculationPayload payload)
    {
        if (payload.FilesCount > 0)
            await RecalculateFilesAsync();

        if (payload.Type is LogType.Api)
            await RecalculateApiAsync(payload);
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

        var query = StatisticsContext.RequestStats
            .Where(o => o.Endpoint == endpoint && (o.SuccessCount + o.FailedCount) > 0)
            .Select(o => (int?)Math.Round(o.TotalDuration / (double)(o.SuccessCount + o.FailedCount)));

        var data = await query.FirstOrDefaultAsync();
        if (data is not null)
            _apiTelemetryCollector.Set(endpoint, data.Value);
    }
}
