using Microsoft.EntityFrameworkCore;
using PointsService.Actions.Merge;
using PointsService.Core.Entity;
using PointsService.Telemetry;

namespace PointsService.Handlers.UserRecalculation;

public partial class UserRecalculationHandler
{
    private async Task ComputeTelemetryAsync(User _)
    {
        var collector = ServiceProvider.GetRequiredService<PointsTelemetryCollector>();

        await ComputeTransactionsMergeTelemetryAsync(collector);
        await ComputeActivePointsAsync(collector);
    }

    private async Task ComputeTransactionsMergeTelemetryAsync(PointsTelemetryCollector collector)
    {
        var action = ServiceProvider.GetRequiredService<MergeValidTransactionsAction>();
        collector.TransactionsToMerge.Set(await action.CountAsync());
    }

    private async Task ComputeActivePointsAsync(PointsTelemetryCollector collector)
    {
        var yearBack = DateTime.UtcNow.AddYears(-1);
        var query = DbContext.Transactions.AsNoTracking().Where(o => o.CreatedAt >= yearBack && o.MergedCount == 0);

        collector.ActiveTransactions.Set(await query.CountAsync());
    }
}
