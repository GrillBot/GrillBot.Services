using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Handlers.Abstractions;
using UserMeasuresService.Models.Events;

namespace UserMeasuresService.Handlers;

public class TimeoutEventHandler : BaseMeasuresHandler<TimeoutPayload>
{
    public TimeoutEventHandler(ILoggerFactory loggerFactory, UserMeasuresContext dbContext, ICounterManager counterManager, IRabbitMQPublisher publisher)
        : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

    protected override async Task HandleInternalAsync(TimeoutPayload payload, Dictionary<string, string> headers)
    {
        var entity = await GetOrCreateEntityAsync(payload.ExternalId);

        entity.CreatedAtUtc = payload.CreatedAtUtc;
        entity.GuildId = payload.GuildId;
        entity.ModeratorId = payload.ModeratorId;
        entity.Reason = payload.Reason;
        entity.UserId = payload.TargetUserId;
        entity.ValidTo = payload.ValidToUtc;

        await SaveEntityAsync(entity);
    }

    private async Task<TimeoutItem> GetOrCreateEntityAsync(long externalId)
    {
        var query = DbContext.Timeouts.Where(o => o.ExternalId == externalId);
        var item = await ContextHelper.ReadFirstOrDefaultEntityAsync(query);

        return item ?? new TimeoutItem { ExternalId = externalId };
    }
}
