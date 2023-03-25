using GrillBot.Core.Infrastructure.Actions;
using PointsService.Core.Repository;
using PointsService.Models;

namespace PointsService.Actions;

public class LeaderboardAction : ApiActionBase
{
    private PointsServiceRepository Repository { get; }

    public LeaderboardAction(PointsServiceRepository repository)
    {
        Repository = repository;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (LeaderboardRequest)Parameters[0]!;

        var result = await Repository.Transaction.ComputeLeaderboardAsync(request.GuildId, request.Skip, request.Count);
        return new ApiResult(StatusCodes.Status200OK, result);
    }
}
