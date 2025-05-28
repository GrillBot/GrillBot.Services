using AuditLogService.Core.Entity.Statistics;
using GrillBot.Core.Metrics.Initializer;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Telemetry.Initializers;

public class JobStatisticsInitializer(
    IServiceProvider serviceProvider,
    AuditLogTelemetryCollector _collector
) : TelemetryInitializer(serviceProvider)
{
    protected override async Task ExecuteInternalAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var context = provider.GetRequiredService<AuditLogStatisticsContext>();

        var statisticsQuery = context.JobInfos.AsNoTracking()
            .Select(o => new
            {
                o.Name,
                Avg = (int)Math.Round(o.TotalDuration / (double)o.StartCount)
            });

        var data = await statisticsQuery.ToListAsync(cancellationToken);
        foreach (var item in data)
            _collector.SetJobsAvgDuration(item.Name, item.Avg);
    }
}
