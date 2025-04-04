﻿using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Messages;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using Microsoft.Extensions.Logging;

namespace GrillBot.Services.Common.Infrastructure.RabbitMQ;

public abstract class BaseEventHandler<TMessage> : RabbitMessageHandlerBase<TMessage> where TMessage : class, IRabbitMessage, new()
{
    protected ICounterManager CounterManager { get; }
    protected IRabbitPublisher Publisher { get; }

    protected string CounterKey { get; }

    protected BaseEventHandler(ILoggerFactory loggerFactory, ICounterManager counterManager, IRabbitPublisher publisher) : base(loggerFactory)
    {
        CounterManager = counterManager;
        Publisher = publisher;
        CounterKey = $"RabbitMQ.{TopicName}.{QueueName}.Consumer";
    }

    protected CounterItem CreateCounter(string operation)
        => CounterManager.Create($"{CounterKey}.{operation}");
}
