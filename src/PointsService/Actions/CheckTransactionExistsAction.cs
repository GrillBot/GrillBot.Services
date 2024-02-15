using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.EntityFrameworkCore;
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

        using (CreateCounter("Database"))
        {
            var exists = await DbContext.Leaderboard.AsNoTracking()
                .AnyAsync(o => o.GuildId == guildId && o.UserId == userId && o.Total > 0);

            return ApiResult.Ok(exists);
        }
    }
}
