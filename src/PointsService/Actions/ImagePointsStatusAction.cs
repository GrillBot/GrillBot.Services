using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using PointsService.Core;
using PointsService.Core.Entity;
using PointsService.Models;

namespace PointsService.Actions;

public class ImagePointsStatusAction : ApiAction
{
    public ImagePointsStatusAction(ICounterManager counterManager, PointsServiceContext dbContext, IRabbitMQPublisher publisher)
        : base(counterManager, dbContext, publisher)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var userId = (string)Parameters[1]!;

        var user = await FindUserAsync(guildId, userId);
        if (user is null)
            return ApiResult.NotFound();

        var result = new ImagePointsStatus
        {
            Position = user.PointsPosition,
            Points = Convert.ToInt32(user.ActivePoints)
        };

        return ApiResult.Ok(result);
    }
}
