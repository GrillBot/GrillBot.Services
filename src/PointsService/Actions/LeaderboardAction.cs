using GrillBot.Core.Infrastructure.Actions;
using PointsService.Core.Repository;
using PointsService.Models;

namespace PointsService.Actions;

public class LeaderboardAction : IApiAction
{
    private PointsServiceRepository Repository { get; }

    public LeaderboardAction(PointsServiceRepository repository)
    {
        Repository = repository;
    }

    public async Task<ApiResult> ProcessAsync(object?[] parameters)
    {
        var request = (LeaderboardRequest)parameters[0]!;

        var result = await Repository.Transaction.ComputeLeaderboardAsync(request.GuildId, request.Skip, request.Count);
        return new ApiResult(StatusCodes.Status200OK, result);
    }
}
