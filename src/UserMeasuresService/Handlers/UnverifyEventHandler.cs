using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Handlers.Abstractions;
using UserMeasuresService.Models.Events;

namespace UserMeasuresService.Handlers;

public class UnverifyEventHandler(
    IServiceProvider serviceProvider
) : BaseMeasuresHandler<UnverifyPayload>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        UnverifyPayload message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers,
        CancellationToken cancellationToken = default
    )
    {
        var entity = new UnverifyItem
        {
            CreatedAtUtc = message.CreatedAtUtc.ToUniversalTime(),
            GuildId = message.GuildId,
            ModeratorId = message.ModeratorId,
            Reason = message.Reason,
            UserId = message.TargetUserId,
            ValidTo = message.EndAtUtc.ToUniversalTime(),
            LogSetId = message.LogSetId
        };

        await SaveEntityAsync(entity);
        return RabbitConsumptionResult.Success;
    }
}
