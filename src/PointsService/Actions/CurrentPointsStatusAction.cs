using GrillBot.Core.Infrastructure.Actions;
using PointsService.Core.Repository;
using PointsService.Models;

namespace PointsService.Actions;

public class CurrentPointsStatusAction : IApiAction
{
    private PointsServiceRepository Repository { get; }

    public CurrentPointsStatusAction(PointsServiceRepository repository)
    {
        Repository = repository;
    }

    public async Task<ApiResult> ProcessAsync(object?[] parameters)
    {
        var guildId = (string)parameters[0]!;
        var userId = (string)parameters[1]!;
        var expired = (bool)parameters[2]!;

        return await ProcessAsync(guildId, userId, expired);
    }

    private async Task<ApiResult> ProcessAsync(string guildId, string userId, bool expired)
    {
        var endOfDay = new TimeSpan(0, 23, 59, 59, 999);
        var now = DateTime.UtcNow;

        var result = new PointsStatus
        {
            Total = await Repository.Transaction.ComputePointsStatusAsync(guildId, userId, expired, DateTime.MinValue, DateTime.MaxValue),
            Today = await Repository.Transaction.ComputePointsStatusAsync(guildId, userId, expired, now.Date, now.Date.Add(endOfDay)),
            MonthBack = await Repository.Transaction.ComputePointsStatusAsync(guildId, userId, expired, now.AddMonths(-1), DateTime.MaxValue),
            YearBack = await Repository.Transaction.ComputePointsStatusAsync(guildId, userId, expired, now.AddYears(-1), DateTime.MaxValue)
        };

        return new ApiResult(StatusCodes.Status200OK, result);
    }
}
