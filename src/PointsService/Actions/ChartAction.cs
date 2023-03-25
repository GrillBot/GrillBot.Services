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

        request.Sort.OrderBy = "Day";
        request.Sort.Descending = false;
        request.MessageId = null;

        var result = await Repository.Transaction.ComputeChartDataAsync(request);

        return new ApiResult(StatusCodes.Status200OK, result);
    }
}
