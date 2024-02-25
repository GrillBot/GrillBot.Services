using GrillBot.Core.Managers.Performance;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Handlers.Abstractions;
using UserMeasuresService.Models.Events;

namespace UserMeasuresService.Handlers;

public class UnverifyEventHandler : BaseEventHandlerWithDb<UnverifyPayload>
{
    public override string QueueName => new UnverifyPayload().QueueName;

    public UnverifyEventHandler(UserMeasuresContext dbContext, ILoggerFactory loggerFactory, ICounterManager counterManager)
        : base(loggerFactory, dbContext, counterManager)
    {
    }

    protected override async Task HandleInternalAsync(UnverifyPayload payload)
    {
        var entity = new UnverifyItem
        {
            CreatedAtUtc = payload.CreatedAt.ToUniversalTime(),
            GuildId = payload.GuildId,
            ModeratorId = payload.ModeratorId,
            Reason = payload.Reason,
            UserId = payload.TargetUserId,
            ValidTo = payload.EndAt.ToUniversalTime(),
            LogSetId = payload.LogSetId
        };

        await SaveEntityAsync(entity);
    }
}
