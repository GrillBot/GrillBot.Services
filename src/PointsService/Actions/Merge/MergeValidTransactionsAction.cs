using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PointsService.Core.Entity;
using PointsService.Core.Options;

namespace PointsService.Actions.Merge;

public class MergeValidTransactionsAction : MergeTransactionsBaseAction
{
    private AppOptions Options { get; }

    private DateTime ExpirationDate { get; set; }

    public MergeValidTransactionsAction(ICounterManager counterManager, PointsServiceContext dbContext, IRabbitMQPublisher publisher, IOptions<AppOptions> options)
        : base(counterManager, dbContext, publisher)
    {
        Options = options.Value;
    }

    protected override Task InitializeAsync()
    {
        ExpirationDate = DateTime.UtcNow.AddMonths(-Options.ExpirationMonths);
        return Task.CompletedTask;
    }

    protected override async Task<bool> CanProcessMergeAsync()
    {
        int expiredCount;
        using (CreateCounter("Database"))
        {
            expiredCount = await DbContext.Transactions.AsNoTracking()
                .CountAsync(o => o.CreatedAt < ExpirationDate && o.MergedCount == 0);
        }

        return expiredCount >= Options.MinimalTransactionsForMerge;
    }

    protected override async Task<List<Transaction>> GetTransactionsForMergeAsync()
    {
        using (CreateCounter("Database"))
        {
            return await DbContext.Transactions
                .Where(o => o.CreatedAt < ExpirationDate && o.MergedCount == 0)
                .ToListAsync();
        }
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
