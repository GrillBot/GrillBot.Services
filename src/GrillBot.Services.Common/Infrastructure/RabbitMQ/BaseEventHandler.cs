using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ;
using GrillBot.Core.RabbitMQ.Consumer;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.Extensions.Logging;

namespace GrillBot.Services.Common.Infrastructure.RabbitMQ;

public abstract class BaseEventHandler<TPayload> : BaseRabbitMQHandler<TPayload> where TPayload : IPayload, new()
{
    public override string QueueName => new TPayload().QueueName;

    protected ICounterManager CounterManager { get; }
    protected IRabbitMQPublisher Publisher { get; }

    protected ILogger Logger { get; }
    protected string CounterKey { get; }

    protected BaseEventHandler(ILoggerFactory loggerFactory, ICounterManager counterManager, IRabbitMQPublisher publisher) : base(loggerFactory)
    {
        CounterManager = counterManager;
        Publisher = publisher;
        Logger = loggerFactory.CreateLogger(GetType());
        CounterKey = $"RabbitMQ.{QueueName}.Consumer";
    }

    protected CounterItem CreateCounter(string operation)
        => CounterManager.Create($"{CounterKey}.{operation}");
}
