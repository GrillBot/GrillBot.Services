using GrillBot.Core.Infrastructure.Actions;
using PointsService.BackgroundServices;
using PointsService.Core.Repository;

namespace PointsService.Actions;

public class DeleteTransactionsAction : ApiActionBase
{
    private PointsServiceRepository Repository { get; }
    private PostProcessingQueue PostProcessingQueue { get; }

    public DeleteTransactionsAction(PointsServiceRepository repository, PostProcessingQueue postProcessingQueue)
    {
        Repository = repository;
        PostProcessingQueue = postProcessingQueue;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var messageId = (string)Parameters[1]!;
        var reactionId = Parameters.ElementAtOrDefault(2) as string;

        return await ProcessAsync(guildId, messageId, reactionId);
    }

    private async Task<ApiResult> ProcessAsync(string guildId, string messageId, string? reactionId)
    {
        var transactions = await Repository.Transaction.FindTransactionsAsync(guildId, messageId, reactionId);
        if (transactions.Count == 0)
            return new ApiResult(StatusCodes.Status204NoContent);

        Repository.RemoveCollection(transactions);
        await Repository.CommitAsync();

        foreach (var transaction in transactions)
            await PostProcessingQueue.SendRequestAsync(transaction, true);

        return new ApiResult(StatusCodes.Status200OK);
    }
}
