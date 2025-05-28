using Microsoft.EntityFrameworkCore;
using PointsService.Actions.Merge;
using PointsService.Core.Entity;

namespace PointsService.Telemetry;

public class PointsTelemetryInitService(
    PointsTelemetryCollector _collector,
    IServiceProvider _serviceProvider
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();

        await InitializeTransctionsToMergeAsync(scope.ServiceProvider);
        await InitializeActiveTransactionsAsync(scope.ServiceProvider);
    }

    private async Task InitializeTransctionsToMergeAsync(IServiceProvider provider)
    {
        var action = provider.GetRequiredService<MergeValidTransactionsAction>();
        _collector.TransactionsToMerge.Set(await action.CountAsync());
    }

    private async Task InitializeActiveTransactionsAsync(IServiceProvider provider)
    {
        var context = provider.GetRequiredService<PointsServiceContext>();
        var yearBack = DateTime.UtcNow.AddYears(-1);
        var query = context.Transactions.AsNoTracking().Where(o => o.CreatedAt >= yearBack && o.MergedCount == 0);

        _collector.ActiveTransactionsCount.Set(await query.CountAsync());
    }
}
