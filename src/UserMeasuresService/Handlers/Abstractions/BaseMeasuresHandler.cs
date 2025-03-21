using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using UserMeasuresService.Core.Entity;

namespace UserMeasuresService.Handlers.Abstractions;

public abstract class BaseMeasuresHandler<TPayload>(
    ILoggerFactory loggerFactory,
    UserMeasuresContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher publisher
) : BaseEventHandlerWithDb<TPayload, UserMeasuresContext>(loggerFactory, dbContext, counterManager, publisher) where TPayload : class, new()
{
    public override string TopicName => "UserMeasures";

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
