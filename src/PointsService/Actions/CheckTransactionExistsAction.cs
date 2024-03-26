using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using PointsService.Core;
using PointsService.Core.Entity;

namespace PointsService.Actions;

public class CheckTransactionExistsAction : ApiAction
{
    public CheckTransactionExistsAction(ICounterManager counterManager, PointsServiceContext dbContext, IRabbitMQPublisher publisher)
        : base(counterManager, dbContext, publisher)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var userId = (string)Parameters[1]!;
        var query = DbContext.Leaderboard.Where(o => o.GuildId == guildId && o.UserId == userId && o.Total > 0);
        var exists = await ContextHelper.IsAnyAsync(query);

        return ApiResult.Ok(exists);
    }
}
