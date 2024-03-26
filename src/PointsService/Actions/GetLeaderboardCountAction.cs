using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using PointsService.Core;
using PointsService.Core.Entity;

namespace PointsService.Actions;

public class GetLeaderboardCountAction : ApiAction
{
    public GetLeaderboardCountAction(ICounterManager counterManager, PointsServiceContext dbContext, IRabbitMQPublisher publisher)
        : base(counterManager, dbContext, publisher)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var query = DbContext.Leaderboard.Where(o => o.GuildId == guildId);
        var result = await ContextHelper.ReadCountAsync(query);

        return ApiResult.Ok(result);
    }
}
