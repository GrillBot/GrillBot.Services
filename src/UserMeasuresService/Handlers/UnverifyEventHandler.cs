using UserMeasuresService.Core.Entity;
using UserMeasuresService.Handlers.Abstractions;
using UserMeasuresService.Models.Events;

namespace UserMeasuresService.Handlers;

public class UnverifyEventHandler : BaseEventHandlerWithDb<UnverifyEvent>
{
    public override string QueueName => UnverifyEvent.QueueName;

    public UnverifyEventHandler(UserMeasuresContext dbContext, ILoggerFactory loggerFactory) : base(loggerFactory, dbContext)
    {
    }

    protected override async Task HandleInternalAsync(UnverifyEvent payload)
    {
        var entity = new UnverifyItem
        {
            CreatedAtUtc = payload.CreatedAt.ToUniversalTime(),
            GuildId = payload.GuildId,
            ModeratorId = payload.ModeratorId,
            Reason = payload.Reason,
            UserId = payload.TargetUserId,
            ValidTo = payload.EndAt.ToUniversalTime()
        };

        await SaveEntityAsync(entity);
    }
}
