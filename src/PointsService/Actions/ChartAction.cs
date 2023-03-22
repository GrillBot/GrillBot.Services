using GrillBot.Core.Infrastructure.Actions;
using PointsService.Core.Repository;
using PointsService.Models;

namespace PointsService.Actions;

public class ChartAction : IApiAction
{
    private PointsServiceRepository Repository { get; }

    public ChartAction(PointsServiceRepository repository)
    {
        Repository = repository;
    }

    public async Task<ApiResult> ProcessAsync(object?[] parameters)
    {
        var request = (AdminListRequest)parameters[0]!;

        request.Sort.OrderBy = "Day";
        request.Sort.Descending = false;
        request.MessageId = null;

        var result = await Repository.Transaction.ComputeChartDataAsync(request);

        return new ApiResult(StatusCodes.Status200OK, result);
    }
}
