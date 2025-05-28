using GrillBot.Core.Metrics.Initializer;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;

namespace PointsService.Telemetry.Initializers;

public class ActiveTransactionsInitializer(
    IServiceProvider serviceProvider,
    PointsTelemetryCollector _collector
) : TelemetryInitializer(serviceProvider)
{
    protected override async Task ExecuteInternalAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var context = provider.GetRequiredService<PointsServiceContext>();
        var yearBack = DateTime.UtcNow.AddYears(-1);
        var query = context.Transactions.AsNoTracking().Where(o => o.CreatedAt >= yearBack && o.MergedCount == 0);

        _collector.ActiveTransactions.Set(await query.CountAsync(cancellationToken));
    }
}
