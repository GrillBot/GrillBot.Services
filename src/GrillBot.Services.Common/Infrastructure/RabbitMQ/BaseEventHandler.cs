using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Messages;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Core.Services.AuditLog.Enums;
using GrillBot.Core.Services.AuditLog.Models.Events.Create;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

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

    protected Task NotifyUnauthorizedExecution(IRabbitMessage message, CancellationToken cancellationToken = default)
    {
        var request = new LogRequest(LogType.Warning, DateTime.UtcNow)
        {
            LogMessage = new LogMessageRequest(
                $"Unauthorized usage of event handler. Missing Authorization token. Topic: {message.Topic}, Queue: {message.Queue}",
                Assembly.GetEntryAssembly()!.GetName().Name!,
                GetType().Name
            )
        };

        return Publisher.PublishAsync(new CreateItemsMessage(request), cancellationToken: cancellationToken);
    }
}
