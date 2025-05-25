
using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using GrillBot.Services.Common.Telemetry.Database;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Telemetry;

public class AuditLogTelemetryInitService(
    IServiceProvider _serviceProvider,
    AuditLogTelemetryCollector _collector,
    DatabaseTelemetryCollector _databaseCollector
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuditLogServiceContext>();
        var statsContext = scope.ServiceProvider.GetRequiredService<AuditLogStatisticsContext>();

        await InitializeFilesAsync(dbContext);
        await InitializeTablesAsync(statsContext);
    }

    private async Task InitializeFilesAsync(AuditLogServiceContext dbContext)
    {
        var baseQuery = dbContext.Files.AsNoTracking();

        _collector.CountOfFiles.Set(await baseQuery.CountAsync());
        _collector.SizeOfFiles.Set(await baseQuery.SumAsync(o => o.Size));
    }

    private async Task InitializeTablesAsync(AuditLogStatisticsContext dbContext)
    {
        var statistics = await dbContext.DatabaseStatistics.AsNoTracking()
            .Select(o => new
            {
                TableName =
                    o.TableName.Contains('.') ?
                    string.Concat(o.TableName.Substring(0, 1).ToLower(), o.TableName.Substring(1)) :
                    o.TableName,
                o.RecordsCount
            })
            .ToDictionaryAsync(o => o.TableName, o => o.RecordsCount);

        foreach (var (table, count) in statistics)
            _databaseCollector.Set(table, (int)count);
    }
}
