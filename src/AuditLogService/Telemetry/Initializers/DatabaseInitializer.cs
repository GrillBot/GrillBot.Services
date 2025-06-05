using AuditLogService.Core.Entity.Statistics;
using GrillBot.Services.Common.Telemetry;
using GrillBot.Services.Common.Telemetry.Database;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Telemetry.Initializers;

public class DatabaseInitializer(
    IServiceProvider serviceProvider,
    DatabaseTelemetryCollector _collector
) : TelemetryInitializerBase(serviceProvider)
{
    protected override async Task ExecuteInternalAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var contextHelper = CreateContextHelper<AuditLogStatisticsContext>(provider);
        var query = contextHelper.DbContext.DatabaseStatistics.AsNoTracking()
            .Select(o => new
            {
                TableName =
                    o.TableName.Contains('.') ?
                    string.Concat(o.TableName.Substring(0, 1).ToLower(), o.TableName.Substring(1)) :
                    o.TableName,
                o.RecordsCount
            });

        var statistics = await contextHelper.ReadToDictionaryAsync(query, o => o.TableName, o => o.RecordsCount);
        foreach (var (table, count) in statistics)
            _collector.SetTableCount(table, (int)count);
    }
}
