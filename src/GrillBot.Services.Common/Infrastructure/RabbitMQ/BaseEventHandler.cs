using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Messages;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GrillBot.Services.Common.Infrastructure.RabbitMQ;

public abstract class BaseEventHandler<TMessage> : RabbitMessageHandlerBase<TMessage> where TMessage : class, IRabbitMessage, new()
{
    protected ICounterManager CounterManager { get; }
    protected IRabbitPublisher Publisher { get; }
    protected IServiceProvider ServiceProvider { get; }

    protected string CounterKey { get; }

    protected BaseEventHandler(IServiceProvider serviceProvider) : base(serviceProvider.GetRequiredService<ILoggerFactory>())
    {
        CounterManager = serviceProvider.GetRequiredService<ICounterManager>();
        Publisher = serviceProvider.GetRequiredService<IRabbitPublisher>();
        ServiceProvider = serviceProvider;

        CounterKey = $"RabbitMQ.{TopicName}.{QueueName}.Consumer";
    }

    protected CounterItem CreateCounter(string operation)
        => CounterManager.Create($"{CounterKey}.{operation}");
}
