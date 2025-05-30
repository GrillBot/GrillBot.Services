using AuditLogService.Core.Entity;
using GrillBot.Core.Metrics.Initializer;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Telemetry.Initializers;

public class ApiRequestCountsByDayInitializer(
    IServiceProvider serviceProvider,
    AuditLogTelemetryCollector _collector
) : TelemetryInitializer(serviceProvider)
{
    protected override async Task ExecuteInternalAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var context = provider.GetRequiredService<AuditLogServiceContext>();

        var query = context.ApiRequests.AsNoTracking()
            .GroupBy(o => new { o.ApiGroupName, o.RequestDate })
            .Select(o => new { o.Key.ApiGroupName, o.Key.RequestDate, Count = o.LongCount() });

        var stats = await query.ToListAsync(cancellationToken);
        foreach (var stat in stats)
            _collector.SetApiRequestCountsByDay(stat.ApiGroupName, stat.RequestDate, stat.Count);
    }
}
