using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using UserMeasuresService.Core.Entity;

namespace UserMeasuresService.Handlers.Abstractions;

public abstract class BaseMeasuresHandler<TPayload> : BaseEventHandlerWithDb<TPayload, UserMeasuresContext> where TPayload : IPayload, new()
{
    protected BaseMeasuresHandler(ILoggerFactory loggerFactory, UserMeasuresContext dbContext, ICounterManager counterManager, IRabbitMQPublisher publisher)
        : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

    protected async Task SaveEntityAsync<TEntity>(TEntity entity) where TEntity : BaseEntity
    {
        if (entity is null)
            return;

        if (entity.IsNew)
        {
            entity.Id = Guid.NewGuid();
            await DbContext.AddAsync(entity);
        }

        await ContextHelper.SaveChagesAsync();
    }
}
