using GrillBot.Core.Infrastructure.Actions;
using PointsService.Core.Repository;
using PointsService.Models;

namespace PointsService.Actions;

public class ChartAction : ApiActionBase
{
    private PointsServiceRepository Repository { get; }

    public ChartAction(PointsServiceRepository repository)
    {
        Repository = repository;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (AdminListRequest)Parameters[0]!;

        request.Sort = null;
        request.MessageId = null;

        if (request.CreatedFrom is null || request.CreatedTo is null)
        {
            var range = await Repository.Transaction.ComputeTransactionDateRangeAsync(request);

            request.CreatedTo = range.to;
            request.CreatedFrom = range.from;
        }

        var from = DateOnly.FromDateTime(request.CreatedFrom.Value);
        var to = DateOnly.FromDateTime(request.CreatedTo.Value);

        var result = await Repository.DailyStats.ComputeChartAsync(from, to, request.GuildId, request.UserId);
        foreach (var item in result)
        {
            if (request.OnlyMessages)
                item.ReactionPoints = 0;

            if (request.OnlyReactions)
                item.MessagePoints = 0;
        }

        return ApiResult.Ok(result);
    }
}
