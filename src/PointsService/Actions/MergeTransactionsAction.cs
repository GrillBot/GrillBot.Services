using Discord;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.Extensions.Options;
using PointsService.Core.Entity;
using PointsService.Core.Options;
using PointsService.Core.Repository;
using PointsService.Models;

namespace PointsService.Actions;

public class MergeTransactionsAction : ApiActionBase
{
    private PointsServiceRepository Repository { get; }
    private AppOptions Options { get; }

    public MergeTransactionsAction(PointsServiceRepository repository, IOptions<AppOptions> options)
    {
        Repository = repository;
        Options = options.Value;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var startAt = DateTime.Now;
        var expirationDate = DateTime.UtcNow.AddMonths(-Options.ExpirationMonths);

        if (!await CanProcessAsync(expirationDate))
            return new ApiResult(StatusCodes.Status204NoContent);

        var transactions = await Repository.Transaction.GetTransactionsForMergeAsync(expirationDate);
        Repository.RemoveCollection(transactions);

        var mergedTransactions = MergeTransactions(transactions);
        await Repository.AddCollectionAsync(mergedTransactions);
        await Repository.CommitAsync();

        var result = new MergeResult
        {
            MergedCount = mergedTransactions.Count,
            Duration = (DateTime.Now - startAt).ToString("c"),
            ExpiredCount = transactions.Count,
            GuildCount = mergedTransactions.DistinctBy(o => o.GuildId).Count(),
            TotalPoints = mergedTransactions.Sum(o => o.Value),
            UserCount = mergedTransactions.DistinctBy(o => o.UserId).Count()
        };

        return new ApiResult(StatusCodes.Status200OK, result);
    }

    private async Task<bool> CanProcessAsync(DateTime expirationDate)
    {
        var count = await Repository.Transaction.GetCountOfTransactionsForMergeAsync(expirationDate);
        return count >= Options.MinimalTransactionsForMerge;
    }

    private static List<Transaction> MergeTransactions(List<Transaction> transactions)
    {
        var result = new List<Transaction>();

        foreach (var transaction in transactions)
        {
            var reactionId = $"Reaction_{transaction.GuildId}_{transaction.UserId}";
            var mergedItems = result.FindAll(o => o.GuildId == transaction.GuildId && o.UserId == transaction.UserId);
            var mergedItem = transaction.IsReaction ? mergedItems.Find(o => o.ReactionId == reactionId) : mergedItems.Find(o => !o.IsReaction);

            if (mergedItem is null)
            {
                mergedItem = new Transaction
                {
                    GuildId = transaction.GuildId,
                    ReactionId = transaction.IsReaction ? reactionId : "",
                    UserId = transaction.UserId,
                    MessageId = SnowflakeUtils.ToSnowflake(DateTimeOffset.Now).ToString(),
                    CreatedAt = DateTime.MaxValue
                };

                result.Add(mergedItem);
            }

            mergedItem.Value += transaction.Value;
            if (transaction.CreatedAt <= (mergedItem.MergeRangeFrom ?? DateTime.MaxValue))
                mergedItem.MergeRangeFrom = transaction.CreatedAt;
            if (transaction.CreatedAt >= (mergedItem.MergeRangeTo ?? DateTime.MinValue))
                mergedItem.MergeRangeTo = transaction.CreatedAt;
            mergedItem.CreatedAt = mergedItem.MergeRangeFrom.GetValueOrDefault();
            mergedItem.MergedCount++;
        }

        return result;
    }
}
