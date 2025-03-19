using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Handlers.Recalculation.Actions;
using AuditLogService.Models.Events.Recalculation;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;

namespace AuditLogService.Handlers.Recalculation;

public class RecalculationHandler(
    ILoggerFactory loggerFactory,
    AuditLogServiceContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher rabbitPublisher,
    IServiceProvider _serviceProvider
) : BaseEventHandlerWithDb<RecalculationPayload, AuditLogServiceContext>(loggerFactory, dbContext, counterManager, rabbitPublisher)
{
    public override string TopicName => "AuditLog";
    public override string QueueName => "Recalculation";

    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        RecalculationPayload message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> headers
    )
    {
        foreach (var action in GetRecalculationActions(message).Where(a => a.CheckPreconditions(message)))
        {
            using (CreateCounter(action.GetType().Name))
                await action.ProcessAsync(message);
        }

        return RabbitConsumptionResult.Success;
    }

    private IEnumerable<RecalculationActionBase> GetRecalculationActions(RecalculationPayload payload)
    {
        if (payload.Type is LogType.Api or LogType.JobCompleted or LogType.InteractionCommand)
        {
            if (payload.Type is LogType.Api)
            {
                yield return new ApiRequestStatsRecalculationAction(_serviceProvider);
                yield return new ApiUserStatsRecalculationAction(_serviceProvider);
            }

            yield return new DailyAvgTimesRecalculationAction(_serviceProvider);

            if (payload.Type is LogType.InteractionCommand)
            {
                yield return new InteractionStatsRecalculationAction(_serviceProvider);
                yield return new InteractionUserStatsRecalculationAction(_serviceProvider);
            }

            if (payload.Type is LogType.JobCompleted)
                yield return new JobInfoRecalculationAction(_serviceProvider);
        }

        yield return new DatabaseStatsRecalculationAction(_serviceProvider);
        yield return new InvalidStatsRecalculationAction(_serviceProvider);
    }
}
