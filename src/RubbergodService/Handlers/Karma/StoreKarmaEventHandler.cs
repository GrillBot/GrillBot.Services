using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using RubbergodService.Core.Entity;
using RubbergodService.Models.Events.Karma;

namespace RubbergodService.Handlers.Karma;

public class StoreKarmaEventHandler : BaseEventHandlerWithDb<KarmaPayload, RubbergodServiceContext>
{
    public StoreKarmaEventHandler(ILoggerFactory loggerFactory, RubbergodServiceContext dbContext, ICounterManager counterManager, IRabbitMQPublisher publisher)
        : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

    protected override async Task HandleInternalAsync(KarmaPayload payload, Dictionary<string, string> headers)
    {
        var entity = await GetOrCreateEntityAsync(payload.MemberId);

        entity.KarmaValue = payload.Karma;
        entity.Positive = payload.Positive;
        entity.Negative = payload.Negative;

        await ContextHelper.SaveChagesAsync();
    }

    private async Task<Core.Entity.Karma> GetOrCreateEntityAsync(string memberId)
    {
        var entity = await ContextHelper.ReadFirstOrDefaultEntityAsync<Core.Entity.Karma>(karma => karma.MemberId == memberId);

        if (entity is null)
        {
            entity = new Core.Entity.Karma { MemberId = memberId };
            await DbContext.AddAsync(entity);
        }

        return entity;
    }
}
