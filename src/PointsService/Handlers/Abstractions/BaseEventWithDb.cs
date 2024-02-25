using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Consumer;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;
using PointsService.Models.Events;

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
        => CounterManager.Create($"RabbitMQ.{QueueName}.Consumer.{operation}");

    protected async Task<User?> FindUserAsync(string guildId, string userId)
    {
        using (CreateCounter("Database"))
            return await DbContext.Users.FirstOrDefaultAsync(o => o.GuildId == guildId && o.Id == userId);
    }

    protected Task EnqueueUserForRecalculationAsync(string guildId, string userId)
        => Publisher.PublishAsync(new UserRecalculationPayload(guildId, userId));
}
