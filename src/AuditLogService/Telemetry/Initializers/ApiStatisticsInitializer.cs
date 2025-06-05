using AuditLogService.Core.Entity.Statistics;
using GrillBot.Services.Common.Telemetry;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Telemetry.Initializers;

public class ApiStatisticsInitializer(
    IServiceProvider serviceProvider,
    AuditLogTelemetryCollector _collector
) : TelemetryInitializerBase(serviceProvider)
{
    protected override async Task ExecuteInternalAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var contextHelper = CreateContextHelper<AuditLogStatisticsContext>(provider);

        var statisticsQuery = contextHelper.DbContext.RequestStats.AsNoTracking()
            .Select(o => new
            {
                o.Endpoint,
                AvgDuration = (int)Math.Round(o.TotalDuration / (double)(o.SuccessCount + o.FailedCount))
            });

        var data = await contextHelper.ReadEntitiesAsync(statisticsQuery);
        foreach (var item in data)
            _collector.SetApiAvgDuration(item.Endpoint, item.AvgDuration);
    }
}
