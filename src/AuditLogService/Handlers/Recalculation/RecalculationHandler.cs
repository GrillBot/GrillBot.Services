using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Handlers.Recalculation.Actions;
using AuditLogService.Models.Events.Recalculation;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;

namespace AuditLogService.Handlers.Recalculation;

public class RecalculationHandler : BaseEventHandlerWithDb<RecalculationPayload, AuditLogServiceContext>
{
    private IServiceProvider ServiceProvider { get; }

    public RecalculationHandler(ILoggerFactory loggerFactory, AuditLogServiceContext dbContext, ICounterManager counterManager,
        IRabbitMQPublisher publisher, IServiceProvider serviceProvider) : base(loggerFactory, dbContext, counterManager, publisher)
    {
        ServiceProvider = serviceProvider;
    }

    protected override async Task HandleInternalAsync(RecalculationPayload payload)
    {
        foreach (var action in GetRecalculationActions(payload))
        {
            using (CreateCounter(action.GetType().Name))
                await action.ProcessAsync(payload);
        }
    }

    private IEnumerable<RecalculationActionBase> GetRecalculationActions(RecalculationPayload payload)
    {
        if (payload.Type is LogType.Api or LogType.JobCompleted or LogType.InteractionCommand)
        {
            if (payload.Type is LogType.Api)
            {
                yield return new ApiRequestStatsRecalculationAction(ServiceProvider);
                yield return new ApiUserStatsRecalculationAction(ServiceProvider);
            }

            yield return new DailyAvtTimesRecalculationAction(ServiceProvider);

            if (payload.Type is LogType.InteractionCommand)
            {
                yield return new InteractionStatsRecalculationAction(ServiceProvider);
                yield return new InteractionUserStatsRecalculationAction(ServiceProvider);
            }

            if (payload.Type is LogType.JobCompleted)
                yield return new JobInfoRecalculationAction(ServiceProvider);
        }

        yield return new DatabaseStatsRecalculationAction(ServiceProvider);
        yield return new InvalidStatsRecalculationAction(ServiceProvider);
    }
}
