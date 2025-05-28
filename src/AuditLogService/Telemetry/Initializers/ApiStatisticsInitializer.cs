using AuditLogService.Core.Entity.Statistics;
using GrillBot.Core.Metrics.Initializer;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Telemetry.Initializers;

public class ApiStatisticsInitializer(
    IServiceProvider serviceProvider,
    AuditLogTelemetryCollector _collector
) : TelemetryInitializer(serviceProvider)
{
    protected override async Task ExecuteInternalAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var context = provider.GetRequiredService<AuditLogStatisticsContext>();
        var statisticsQuery = context.RequestStats.AsNoTracking()
            .Select(o => new
            {
                o.Endpoint,
                AvgDuration = (int)Math.Round(o.TotalDuration / (double)(o.SuccessCount + o.FailedCount))
            });

        var data = await statisticsQuery.ToListAsync(cancellationToken);
        foreach (var item in data)
            _collector.SetApiAvgDuration(item.Endpoint, item.AvgDuration);
    }
}
