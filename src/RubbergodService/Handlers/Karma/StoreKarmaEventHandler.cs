using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using RubbergodService.Core.Entity;
using RubbergodService.Models.Events.Karma;

namespace RubbergodService.Handlers.Karma;

public class StoreKarmaEventHandler : BaseEventHandlerWithDb<KarmaBatchPayload, RubbergodServiceContext>
{
    public StoreKarmaEventHandler(ILoggerFactory loggerFactory, RubbergodServiceContext dbContext, ICounterManager counterManager, IRabbitMQPublisher publisher)
        : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

    protected override async Task HandleInternalAsync(KarmaBatchPayload payload, Dictionary<string, string> headers)
    {
        foreach (var chunk in payload.Users.Where(o => !string.IsNullOrEmpty(o.MemberId)).Chunk(50))
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
