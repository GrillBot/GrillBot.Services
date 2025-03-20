using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PointsService.Core;
using PointsService.Core.Entity;
using PointsService.Core.Options;
using PointsService.Models;

namespace PointsService.Actions;

public class GetStatusInfoAction(
    ICounterManager counterManager,
    PointsServiceContext dbContext,
    IRabbitPublisher publisher,
    IOptions<AppOptions> _options
) : ApiAction(counterManager, dbContext, publisher)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var result = new StatusInfo
        {
            TransactionsToMerge = await ComputeTransactionsToMergeAsync()
        };

        return ApiResult.Ok(result);
    }

    private async Task<int> ComputeTransactionsToMergeAsync()
    {
        var expirationDate = DateTime.UtcNow.AddMonths(-_options.Value.ExpirationMonths);

        var query = DbContext.Transactions.AsNoTracking()
            .Where(o => o.CreatedAt < expirationDate && o.MergedCount == 0);

        return await ContextHelper.ReadCountAsync(query);
    }
}
