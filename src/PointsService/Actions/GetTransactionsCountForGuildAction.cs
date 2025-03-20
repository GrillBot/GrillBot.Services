using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using PointsService.Core;
using PointsService.Core.Entity;

namespace PointsService.Actions;

public class GetTransactionsCountForGuildAction(
    ICounterManager counterManager,
    PointsServiceContext dbContext,
    IRabbitPublisher publisher
) : ApiAction(counterManager, dbContext, publisher)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var query = DbContext.Transactions.Where(o => o.GuildId == guildId);
        var result = await ContextHelper.ReadCountAsync(query);

        return ApiResult.Ok(result);
    }
}
