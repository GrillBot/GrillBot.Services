using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using PointsService.Core.Entity;
using PointsService.Handlers.Abstractions;
using PointsService.Models.Events;

namespace PointsService.Handlers;

public class DeleteTransactionsEventHandler(
    IServiceProvider serviceProvider
) : BasePointsEvent<DeleteTransactionsPayload>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(DeleteTransactionsPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        var transactions = await ReadTransactionsAsync(message);
        if (transactions.Count == 0)
            return RabbitConsumptionResult.Success;

        DbContext.RemoveRange(transactions);
        await ContextHelper.SaveChagesAsync();
        await EnqueueUserForRecalculationAsync(transactions);
        return RabbitConsumptionResult.Success;
    }

    private async Task<List<Transaction>> ReadTransactionsAsync(DeleteTransactionsPayload payload)
    {
        var query = DbContext.Transactions
            .Where(o => o.GuildId == payload.GuildId && o.MergedCount == 0 && o.MessageId == payload.MessageId);
        if (!string.IsNullOrEmpty(payload.ReactionId))
            query = query.Where(o => o.ReactionId == payload.ReactionId);

        return await ContextHelper.ReadEntitiesAsync(query);
    }

    private Task EnqueueUserForRecalculationAsync(List<Transaction> transactions)
    {
        var users = transactions
            .GroupBy(o => new { o.GuildId, o.UserId })
            .Select(o => (o.Key.GuildId, o.Key.UserId));

        return EnqueueUsersForRecalculationAsync(users);
    }
}
