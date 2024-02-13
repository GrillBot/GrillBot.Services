using GrillBot.Core.Infrastructure.Actions;
using PointsService.Core.Repository;

namespace PointsService.Actions;

public class LeaderboardCountAction : ApiActionBase
{
    private PointsServiceRepository Repository { get; }

    public LeaderboardCountAction(PointsServiceRepository repository)
    {
        Repository = repository;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var result = await Repository.Leaderboard.ComputeLeaderboardCountAsync(guildId);

        return ApiResult.Ok(result);
    }
}
