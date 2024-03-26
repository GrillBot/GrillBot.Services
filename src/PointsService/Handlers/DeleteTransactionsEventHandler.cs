using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using PointsService.Core.Entity;
using PointsService.Handlers.Abstractions;
using PointsService.Models.Events;

namespace PointsService.Handlers;

public class DeleteTransactionsEventHandler : BasePointsEvent<DeleteTransactionsPayload>
{
    public DeleteTransactionsEventHandler(ILoggerFactory loggerFactory, PointsServiceContext dbContext, ICounterManager counterManager, IRabbitMQPublisher publisher)
        : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

    protected override async Task HandleInternalAsync(DeleteTransactionsPayload payload)
    {
        var transactions = await ReadTransactionsAsync(payload);
        if (transactions.Count == 0)
            return;

        DbContext.RemoveRange(transactions);
        await ContextHelper.SaveChagesAsync();
        await EnqueueUserForRecalculationAsync(transactions);
    }

    private async Task<List<Transaction>> ReadTransactionsAsync(DeleteTransactionsPayload payload)
    {
        var query = DbContext.Transactions
            .Where(o => o.GuildId == payload.GuildId && o.MergedCount == 0 && o.MessageId == payload.MessageId);
        if (!string.IsNullOrEmpty(payload.ReactionId))
            query = query.Where(o => o.ReactionId == payload.ReactionId);

        return await ContextHelper.ReadEntitiesAsync(query);
    }

    private async Task EnqueueUserForRecalculationAsync(List<Transaction> transactions)
    {
        var users = transactions
            .GroupBy(o => new { o.GuildId, o.UserId })
            .Select(o => o.First());

        foreach (var userTransaction in users)
            await Publisher.PublishAsync(new UserRecalculationPayload(userTransaction.GuildId, userTransaction.UserId));
    }
}
