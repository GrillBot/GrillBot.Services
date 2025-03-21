using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Handlers.Abstractions;
using UserMeasuresService.Models.Events;

namespace UserMeasuresService.Handlers;

public class UnverifyEventHandler(
    UserMeasuresContext dbContext,
    ILoggerFactory loggerFactory,
    ICounterManager counterManager,
    IRabbitPublisher publisher
) : BaseMeasuresHandler<UnverifyPayload>(loggerFactory, dbContext, counterManager, publisher)
{
    public override string QueueName => "CreateUnverify";

    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(UnverifyPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
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
