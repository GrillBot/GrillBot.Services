using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Consumer;
using UserMeasuresService.Core.Entity;

namespace UserMeasuresService.Handlers.Abstractions;

public abstract class BaseEventHandlerWithDb<TPayload> : BaseRabbitMQHandler<TPayload>
{
    protected UserMeasuresContext DbContext { get; }
    private ICounterManager CounterManager { get; }

    protected BaseEventHandlerWithDb(ILoggerFactory loggerFactory, UserMeasuresContext dbContext, ICounterManager counterManager) : base(loggerFactory)
    {
        DbContext = dbContext;
        CounterManager = counterManager;
    }

    protected async Task SaveEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        if (entity is null)
            return;

        using (CreateCounter("Database"))
        {
            if (entity.IsNew)
            {
                entity.Id = Guid.NewGuid();
                await DbContext.AddAsync(entity);
            }

            await DbContext.SaveChangesAsync();
        }
    }

    protected CounterItem CreateCounter(string operation)
        => CounterManager.Create($"RabbitMQ.{QueueName}.Consumer.{operation}");
}
