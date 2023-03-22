using GrillBot.Core.Infrastructure.Actions;
using PointsService.Core.Repository;

namespace PointsService.Actions;

public class DeleteTransactionsAction : IApiAction
{
    private PointsServiceRepository Repository { get; }

    public DeleteTransactionsAction(PointsServiceRepository repository)
    {
        Repository = repository;
    }

    public async Task<ApiResult> ProcessAsync(object?[] parameters)
    {
        var guildId = (string)parameters[0]!;
        var messageId = (string)parameters[1]!;
        var reactionId = parameters.ElementAtOrDefault(2) as string;

        return await ProcessAsync(guildId, messageId, reactionId);
    }

    private async Task<ApiResult> ProcessAsync(string guildId, string messageId, string? reactionId)
    {
        var transactions = await Repository.Transaction.FindTransactionsAsync(guildId, messageId, reactionId);
        if (transactions.Count == 0)
            return new ApiResult(StatusCodes.Status204NoContent);

        Repository.RemoveCollection(transactions);
        await Repository.CommitAsync();
        return new ApiResult(StatusCodes.Status200OK);
    }
}
