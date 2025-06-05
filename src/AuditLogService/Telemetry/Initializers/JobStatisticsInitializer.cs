using AuditLogService.Core.Entity.Statistics;
using GrillBot.Services.Common.Telemetry;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Telemetry.Initializers;

public class JobStatisticsInitializer(
    IServiceProvider serviceProvider,
    AuditLogTelemetryCollector _collector
) : TelemetryInitializerBase(serviceProvider)
{
    protected override async Task ExecuteInternalAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var contextHelper = CreateContextHelper<AuditLogStatisticsContext>(provider);

        var statisticsQuery = contextHelper.DbContext.JobInfos.AsNoTracking()
            .Select(o => new
            {
                o.Name,
                Avg = (int)Math.Round(o.TotalDuration / (double)o.StartCount)
            });

        var data = await contextHelper.ReadEntitiesAsync(statisticsQuery);
        foreach (var item in data)
            _collector.SetJobsAvgDuration(item.Name, item.Avg);
    }
}
