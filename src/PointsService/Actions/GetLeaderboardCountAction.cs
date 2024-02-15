using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.EntityFrameworkCore;
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

        using (CreateCounter("Database"))
        {
            var count = await DbContext.Leaderboard.AsNoTracking()
                .CountAsync(o => o.GuildId == guildId);

            return ApiResult.Ok(count);
        }
    }
}
