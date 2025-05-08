using GrillBot.Core.RabbitMQ.V2.Messages;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using UserMeasuresService.Core.Entity;

namespace UserMeasuresService.Handlers.Abstractions;

public abstract class BaseMeasuresHandler<TPayload>(
    IServiceProvider serviceProvider
) : BaseEventHandlerWithDb<TPayload, UserMeasuresContext>(serviceProvider) where TPayload : class, IRabbitMessage, new()
{
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
