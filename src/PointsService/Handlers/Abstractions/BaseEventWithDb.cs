using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Consumer;
using GrillBot.Core.RabbitMQ.Publisher;
using PointsService.Core.Entity;

namespace PointsService.Handlers.Abstractions;

public abstract class BaseEventWithDb<TPayload> : BaseRabbitMQHandler<TPayload>
{
    protected PointsServiceContext DbContext { get; }
    private ICounterManager CounterManager { get; }
    protected IRabbitMQPublisher Publisher { get; }

    protected BaseEventWithDb(ILoggerFactory loggerFactory, PointsServiceContext dbContext, ICounterManager counterManager,
        IRabbitMQPublisher publisher) : base(loggerFactory)
    {
        DbContext = dbContext;
        CounterManager = counterManager;
        Publisher = publisher;
    }

    protected CounterItem CreateCounter(string operation)
        => CounterManager.Create($"RabbitMQ.{QueueName}.{operation}");
}
