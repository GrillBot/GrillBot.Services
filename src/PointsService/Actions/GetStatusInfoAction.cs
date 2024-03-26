using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PointsService.Core;
using PointsService.Core.Entity;
using PointsService.Core.Options;
using PointsService.Models;

namespace PointsService.Actions;

public class GetStatusInfoAction : ApiAction
{
    private AppOptions Options { get; }

    public GetStatusInfoAction(ICounterManager counterManager, PointsServiceContext dbContext, IRabbitMQPublisher publisher,
        IOptions<AppOptions> options) : base(counterManager, dbContext, publisher)
    {
        Options = options.Value;
    }

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
        var expirationDate = DateTime.UtcNow.AddMonths(-Options.ExpirationMonths);

        var query = DbContext.Transactions.AsNoTracking()
            .Where(o => o.CreatedAt < expirationDate && o.MergedCount == 0);

        return await ContextHelper.ReadCountAsync(query);
    }
}
