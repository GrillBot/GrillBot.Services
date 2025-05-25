
using AuditLogService.Core.Entity;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Telemetry;

public class AuditLogTelemetryInitService(
    IServiceProvider _serviceProvider,
    AuditLogTelemetryCollector _collector
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuditLogServiceContext>();

        await InitializeFilesAsync(dbContext);
    }

    private async Task InitializeFilesAsync(AuditLogServiceContext dbContext)
    {
        var baseQuery = dbContext.Files.AsNoTracking();

        _collector.CountOfFiles.Set(await baseQuery.CountAsync());
        _collector.SizeOfFiles.Set(await baseQuery.SumAsync(o => o.Size));
    }
}
