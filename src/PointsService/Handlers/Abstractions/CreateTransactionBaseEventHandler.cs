using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using PointsService.Core.Entity;
using PointsService.Models.Events;

namespace PointsService.Handlers.Abstractions;

public abstract class CreateTransactionBaseEventHandler<TPayload> : BaseEventWithDb<TPayload> where TPayload : CreateTransactionBasePayload
{
    protected ILogger Logger { get; }

    protected CreateTransactionBaseEventHandler(ILoggerFactory loggerFactory, PointsServiceContext dbContext, ICounterManager counterManager,
        IRabbitMQPublisher publisher) : base(loggerFactory, dbContext, counterManager, publisher)
    {
        Logger = loggerFactory.CreateLogger(GetType());
    }

    protected bool ValidationFailed(string message, bool suppressAudit = false)
    {
        var eventId = suppressAudit ?
            new EventId(1, "ValidationFailed_SuppressAudit") :
            new EventId(2, "ValidationFailed_PublishAudit");

        Logger.LogWarning(eventId, "{message}", message);
        return false;
    }

    protected async Task CommitTransactionAsync(Transaction transaction)
    {
        using (CreateCounter("Database"))
        {
            await DbContext.AddAsync(transaction);
            await DbContext.SaveChangesAsync();
        }
    }
}
