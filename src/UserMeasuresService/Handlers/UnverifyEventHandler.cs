using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Handlers.Abstractions;
using UserMeasuresService.Models.Events;

namespace UserMeasuresService.Handlers;

public class UnverifyEventHandler : BaseMeasuresHandler<UnverifyPayload>
{
    public UnverifyEventHandler(UserMeasuresContext dbContext, ILoggerFactory loggerFactory, ICounterManager counterManager, IRabbitMQPublisher publisher)
        : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

    protected override async Task HandleInternalAsync(UnverifyPayload payload, Dictionary<string, string> headers)
    {
        var entity = new UnverifyItem
        {
            CreatedAtUtc = payload.CreatedAtUtc.ToUniversalTime(),
            GuildId = payload.GuildId,
            ModeratorId = payload.ModeratorId,
            Reason = payload.Reason,
            UserId = payload.TargetUserId,
            ValidTo = payload.EndAtUtc.ToUniversalTime(),
            LogSetId = payload.LogSetId
        };

        await SaveEntityAsync(entity);
    }
}
