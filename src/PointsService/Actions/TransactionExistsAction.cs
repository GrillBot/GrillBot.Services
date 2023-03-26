using GrillBot.Core.Infrastructure.Actions;
using PointsService.Core.Repository;

namespace PointsService.Actions;

public class TransactionExistsAction : ApiActionBase
{
    private PointsServiceRepository Repository { get; }

    public TransactionExistsAction(PointsServiceRepository repository)
    {
        Repository = repository;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var userId = (string)Parameters[1]!;

        var result = await Repository.Transaction.ExistsAnyTransactionAsync(guildId, userId);
        return new ApiResult(StatusCodes.Status200OK, result);
    }
}
