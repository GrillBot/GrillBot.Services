using GrillBot.Core.RabbitMQ.Consumer;
using UserMeasuresService.Core.Entity;

namespace UserMeasuresService.Handlers.Abstractions;

public abstract class BaseEventHandlerWithDb<TPayload> : BaseRabbitMQHandler<TPayload>
{
    protected UserMeasuresContext DbContext { get; }

    protected BaseEventHandlerWithDb(ILoggerFactory loggerFactory, UserMeasuresContext dbContext) : base(loggerFactory)
    {
        DbContext = dbContext;
    }

    protected async Task SaveEntityAsync<TEntity>(TEntity entity) where TEntity : class
    {
        if (entity is null)
            return;

        await DbContext.AddAsync(entity);
        await DbContext.SaveChangesAsync();
    }
}
