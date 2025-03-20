using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using Microsoft.Extensions.Options;
using PointsService.Core.Entity;
using PointsService.Core.Options;

namespace PointsService.Actions.Merge;

public class MergeValidTransactionsAction(
    ICounterManager counterManager,
    PointsServiceContext dbContext,
    IRabbitPublisher publisher,
    IOptions<AppOptions> _options
) : MergeTransactionsBaseAction(counterManager, dbContext, publisher)
{
    private DateTime ExpirationDate { get; set; }

    protected override Task InitializeAsync()
    {
        ExpirationDate = DateTime.UtcNow.AddMonths(-_options.Value.ExpirationMonths);
        return Task.CompletedTask;
    }

    protected override async Task<bool> CanProcessMergeAsync()
    {
        var query = DbContext.Transactions.Where(o => o.CreatedAt < ExpirationDate && o.MergedCount == 0);
        var expiredCount = await ContextHelper.ReadCountAsync(query);

        return expiredCount >= _options.Value.MinimalTransactionsForMerge;
    }

    protected override async Task<List<Transaction>> GetTransactionsForMergeAsync()
    {
        var query = DbContext.Transactions.Where(o => o.CreatedAt < ExpirationDate && o.MergedCount == 0);
        return await ContextHelper.ReadEntitiesAsync(query);
    }

    protected override List<Transaction> ProcessMergeInternal(List<Transaction> transactions)
    {
        var result = new List<Transaction>();

        foreach (var transaction in transactions)
        {
            var reactionId = $"Reaction_{transaction.GuildId}_{transaction.UserId}";
            var mergedItems = result.FindAll(o => o.GuildId == transaction.GuildId && o.UserId == transaction.UserId);
            var mergedItem = transaction.IsReaction ? mergedItems.Find(o => o.ReactionId == reactionId) : mergedItems.Find(o => !o.IsReaction);

            if (mergedItem is null)
            {
                mergedItem = CloneTransaction(transaction);
                if (transaction.IsReaction)
                    mergedItem.ReactionId = reactionId;

                result.Add(mergedItem);
            }

            MergeTransactionProperties(mergedItem, transaction);
        }

        return result;
    }
}
