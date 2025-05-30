using AuditLogService.Models.Events.Recalculation;
using AuditLogService.Telemetry;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Handlers.Recalculation.Actions.Telemetry;

public class RecalculateApiTelemetryAction(IServiceProvider serviceProvider) : RecalculationActionBase(serviceProvider)
{
    private readonly AuditLogTelemetryCollector _telemetryCollector
        = serviceProvider.GetRequiredService<AuditLogTelemetryCollector>();

    public override bool CheckPreconditions(RecalculationPayload payload)
        => payload.Api is not null;

    public override async Task ProcessAsync(RecalculationPayload payload)
    {
        await RecalculateAvgDurationAsync(payload);
        await RecalculateDailyCountsAsync(payload);
    }

    private async Task RecalculateAvgDurationAsync(RecalculationPayload payload)
    {
        var endpoint = $"{payload.Api!.Method} {payload.Api.TemplatePath}";
        var query = StatisticsContext.RequestStats.AsNoTracking()
            .Where(o => o.Endpoint == endpoint)
            .Select(o => (int?)Math.Round(o.TotalDuration / (double)(o.SuccessCount + o.FailedCount)));

        var data = await query.FirstOrDefaultAsync();
        if (data is not null)
            _telemetryCollector.SetApiAvgDuration(endpoint, data.Value);
    }

    private async Task RecalculateDailyCountsAsync(RecalculationPayload payload)
    {
        var query = DbContext.ApiRequests.AsNoTracking()
            .Where(o => o.ApiGroupName == payload.Api!.ApiGroupName && o.RequestDate == payload.Api.RequestDate);

        var count = await query.LongCountAsync();
        _telemetryCollector.SetApiRequestCountsByDay(payload.Api!.ApiGroupName, payload.Api.RequestDate, count);
    }
}
