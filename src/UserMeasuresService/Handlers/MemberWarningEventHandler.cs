using UserMeasuresService.Core.Entity;
using UserMeasuresService.Handlers.Abstractions;
using UserMeasuresService.Models.Events;

namespace UserMeasuresService.Handlers;

public class MemberWarningEventHandler : BaseEventHandlerWithDb<MemberWarningPayload>
{
    public override string QueueName => MemberWarningPayload.QueueName;

    public MemberWarningEventHandler(ILoggerFactory loggerFactory, UserMeasuresContext dbContext) : base(loggerFactory, dbContext)
    {
    }

    protected override async Task HandleInternalAsync(MemberWarningPayload payload)
    {
        var entity = new MemberWarningItem
        {
            CreatedAtUtc = payload.CreatedAt.ToUniversalTime(),
            GuildId = payload.GuildId,
            ModeratorId = payload.ModeratorId,
            Reason = payload.Reason,
            UserId = payload.TargetUserId
        };

        await SaveEntityAsync(entity);
    }
}
