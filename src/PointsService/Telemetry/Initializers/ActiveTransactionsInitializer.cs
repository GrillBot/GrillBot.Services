using GrillBot.Services.Common.Telemetry;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;

namespace PointsService.Telemetry.Initializers;

public class ActiveTransactionsInitializer(
    IServiceProvider serviceProvider,
    PointsTelemetryCollector _collector
) : TelemetryInitializerBase(serviceProvider)
{
    protected override async Task ExecuteInternalAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var yearBack = DateTime.UtcNow.AddYears(-1);
        var contextHelper = CreateContextHelper<PointsServiceContext>(provider);

        var query = contextHelper.DbContext.Transactions.AsNoTracking()
            .Where(o => o.CreatedAt >= yearBack && o.MergedCount == 0);

        _collector.ActiveTransactions.Set(await contextHelper.ReadCountAsync(query));
    }
}
