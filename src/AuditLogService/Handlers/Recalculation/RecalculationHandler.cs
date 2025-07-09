using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Handlers.Recalculation.Actions;
using AuditLogService.Handlers.Recalculation.Actions.Telemetry;
using AuditLogService.Models.Events.Recalculation;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;

namespace AuditLogService.Handlers.Recalculation;

public class RecalculationHandler(IServiceProvider serviceProvider)
    : BaseEventHandlerWithDb<RecalculationPayload, AuditLogServiceContext>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(
        RecalculationPayload message,
        ICurrentUserProvider currentUser,
        Dictionary<string, string> header,
        CancellationToken cancellationToken = default
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
                yield return new ApiRequestStatsRecalculationAction(ServiceProvider);
                yield return new ApiUserStatsRecalculationAction(ServiceProvider);
            }

            yield return new DailyAvgTimesRecalculationAction(ServiceProvider);

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
        yield return new RecalculateLogTelemetryAction(ServiceProvider);

        if (payload.FilesCount > 0)
            yield return new RecalculateFilesTelemetryAction(ServiceProvider);

        if (payload.Type is LogType.Api)
        {
            yield return new RecalculateApiTelemetryAction(ServiceProvider);
        }

        if (payload.Type is LogType.JobCompleted)
        {
            yield return new RecalculateJobsTelemetryAction(ServiceProvider);
        }
    }
}
