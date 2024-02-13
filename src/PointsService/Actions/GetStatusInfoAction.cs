using GrillBot.Core.Infrastructure.Actions;
using Microsoft.Extensions.Options;
using PointsService.Core.Options;
using PointsService.Core.Repository;
using PointsService.Models;

namespace PointsService.Actions;

public class GetStatusInfoAction : ApiActionBase
{
    private PointsServiceRepository Repository { get; }
    private AppOptions Options { get; }

    public GetStatusInfoAction(PointsServiceRepository repository, IOptions<AppOptions> options)
    {
        Repository = repository;
        Options = options.Value;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var expirationDate = DateTime.UtcNow.AddMonths(-Options.ExpirationMonths);

        var result = new StatusInfo
        {
            PendingUsersToProcess = await Repository.User.CountUsersToProcessAsync(),
            TransactionsToMerge = await Repository.Transaction.GetCountOfTransactionsForMergeAsync(expirationDate)
        };

        return ApiResult.Ok(result);
    }
}
