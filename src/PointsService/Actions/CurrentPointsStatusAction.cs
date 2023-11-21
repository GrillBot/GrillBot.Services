using GrillBot.Core.Infrastructure.Actions;
using PointsService.Core.Repository;
using PointsService.Models;

namespace PointsService.Actions;

public class CurrentPointsStatusAction : ApiActionBase
{
    private PointsServiceRepository Repository { get; }

    public CurrentPointsStatusAction(PointsServiceRepository repository)
    {
        Repository = repository;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var userId = (string)Parameters[1]!;
        var expired = (bool)Parameters[2]!;

        return await ProcessAsync(guildId, userId, expired);
    }

    private async Task<ApiResult> ProcessAsync(string guildId, string userId, bool expired)
    {
        var result = expired ?
            await ComputeStatusOfExpiredPoints(guildId, userId) :
            await Repository.Transaction.ComputePointsStatusAsync(guildId, userId, expired);

        return ApiResult.FromSuccess(result);
    }

    private async Task<PointsStatus> ComputeStatusOfExpiredPoints(string guildId, string userId)
    {
        return new PointsStatus
        {
            Total = await Repository.Transaction.ComputePointsStatusAsync(guildId, userId, true, DateTime.MinValue, DateTime.MaxValue)
        };
    }
}
