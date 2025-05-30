using AuditLogService.Core.Options;
using AuditLogService.Models.Events.Recalculation;
using AuditLogService.Telemetry;
using GrillBot.Services.Common.EntityFramework.Extensions;
using Microsoft.Extensions.Options;

namespace AuditLogService.Handlers.Recalculation.Actions.Telemetry;

public class RecalculateLogTelemetryAction(IServiceProvider serviceProvider) : RecalculationActionBase(serviceProvider)
{
    private readonly AuditLogTelemetryCollector _telemetryCollector
        = serviceProvider.GetRequiredService<AuditLogTelemetryCollector>();

    private readonly AppOptions _options
        = serviceProvider.GetRequiredService<IOptions<AppOptions>>().Value;

    public override async Task ProcessAsync(RecalculationPayload payload)
    {
        await RecalculateItemsToArchiveAsync();
    }

    private async Task RecalculateItemsToArchiveAsync()
    {
        var expirationDate = DateTime.UtcNow.AddMonths(-_options.ExpirationMonths);
        var query = DbContext.LogItems.Where(o => o.CreatedAt <= expirationDate || o.IsDeleted);
        var count = await query.CountAsync();

        _telemetryCollector.ItemsOfArchive.Set(count);
    }
}
