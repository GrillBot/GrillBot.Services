using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using RubbergodService.Core.Entity;
using RubbergodService.Models.Events.Karma;

namespace RubbergodService.Handlers.Karma;

public class StoreKarmaEventHandler(
    ILoggerFactory loggerFactory,
    RubbergodServiceContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher publisher
) : BaseEventHandlerWithDb<KarmaBatchPayload, RubbergodServiceContext>(loggerFactory, dbContext, counterManager, publisher)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(KarmaBatchPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        foreach (var chunk in message.Users.Where(o => !string.IsNullOrEmpty(o.MemberId)).Chunk(50))
        {
            var entities = await GetOrCreateEntitiesAsync(chunk.Select(o => o.MemberId).ToList());

            foreach (var user in chunk)
            {
                if (!entities.TryGetValue(user.MemberId, out var entity))
                    continue;

                entity.KarmaValue = user.Karma;
                entity.Positive = user.Positive;
                entity.Negative = user.Negative;
            }
        }

        await ContextHelper.SaveChagesAsync();
        return RabbitConsumptionResult.Success;
    }

    private async Task<Dictionary<string, Core.Entity.Karma>> GetOrCreateEntitiesAsync(List<string> memberIds)
    {
        var query = DbContext.Karma.Where(o => memberIds.Contains(o.MemberId));
        var entities = await ContextHelper.ReadEntitiesAsync(query);

        if (entities.Count == memberIds.Count)
            return entities.ToDictionary(o => o.MemberId, o => o);

        foreach (var memberId in memberIds.Where(id => !entities.Exists(e => e.MemberId == id)))
        {
            var entity = new Core.Entity.Karma { MemberId = memberId };

            entities.Add(entity);
            await DbContext.AddAsync(entity);
        }

        return entities.ToDictionary(o => o.MemberId, o => o);
    }
}
