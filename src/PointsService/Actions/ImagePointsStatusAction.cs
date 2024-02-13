using GrillBot.Core.Infrastructure.Actions;
using PointsService.Core.Repository;
using PointsService.Models;

namespace PointsService.Actions;

public class ImagePointsStatusAction : ApiActionBase
{
    private PointsServiceRepository Repository { get; }

    public ImagePointsStatusAction(PointsServiceRepository repository)
    {
        Repository = repository;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var userId = (string)Parameters[1]!;

        return await ProcessAsync(guildId, userId);
    }

    private async Task<ApiResult> ProcessAsync(string guildId, string userId)
    {
        var now = DateTime.UtcNow;
        var yearBack = now.AddYears(-1);

        var user = await Repository.User.FindUserAsync(guildId, userId, true);
        if (user is null)
            return new ApiResult(StatusCodes.Status404NotFound);

        var points = await Repository.Transaction.ComputePointsStatusAsync(guildId, userId, false, yearBack, DateTime.MaxValue);
        var result = new ImagePointsStatus
        {
            Points = points,
            Position = user!.PointsPosition
        };

        return ApiResult.Ok(result);
    }
}
