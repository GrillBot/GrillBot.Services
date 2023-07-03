using GrillBot.Core.Infrastructure.Actions;
using PointsService.Core.Entity;
using PointsService.Core.Repository;

namespace PointsService.Actions;

public class DeleteTransactionsAction : ApiActionBase
{
    private PointsServiceRepository Repository { get; }

    public DeleteTransactionsAction(PointsServiceRepository repository)
    {
        Repository = repository;
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
        await SetPendingStatusAsync(transactions);
        await Repository.CommitAsync();

        return ApiResult.FromSuccess();
    }

    private async Task SetPendingStatusAsync(IEnumerable<Transaction> transactions)
    {
        var users = transactions
            .GroupBy(o => new { o.GuildId, o.UserId })
            .Select(o => o.First())
            .ToList();
        foreach (var user in users)
        {
            var entity = await Repository.User.FindUserAsync(user.GuildId, user.UserId);
            if (entity is null) continue;

            entity.PendingRecalculation = true;
        }
    }
}
