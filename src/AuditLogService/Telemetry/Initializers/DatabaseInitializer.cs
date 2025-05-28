using AuditLogService.Core.Entity.Statistics;
using GrillBot.Core.Metrics.Initializer;
using GrillBot.Services.Common.Telemetry.Database;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Telemetry.Initializers;

public class DatabaseInitializer(
    IServiceProvider serviceProvider,
    DatabaseTelemetryCollector _collector
) : TelemetryInitializer(serviceProvider)
{
    protected override async Task ExecuteInternalAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var context = provider.GetRequiredService<AuditLogStatisticsContext>();
        var statistics = await context.DatabaseStatistics.AsNoTracking()
            .Select(o => new
            {
                TableName =
                    o.TableName.Contains('.') ?
                    string.Concat(o.TableName.Substring(0, 1).ToLower(), o.TableName.Substring(1)) :
                    o.TableName,
                o.RecordsCount
            })
            .ToDictionaryAsync(o => o.TableName, o => o.RecordsCount, cancellationToken);

        foreach (var (table, count) in statistics)
            _collector.SetTableCount(table, (int)count);
    }
}
