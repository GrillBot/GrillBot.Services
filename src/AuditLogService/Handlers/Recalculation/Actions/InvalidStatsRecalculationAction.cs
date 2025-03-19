using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Events.Recalculation;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Handlers.Recalculation.Actions;

public class InvalidStatsRecalculationAction(IServiceProvider serviceProvider) : RecalculationActionBase(serviceProvider)
{
    public override async Task ProcessAsync(RecalculationPayload payload)
    {
        if (payload.Type is LogType.Api or LogType.InteractionCommand or LogType.JobCompleted)
        {
            if (payload.Type is LogType.Api)
            {
                await ClearStatisticsAsync<ApiRequestStat>(o => o.FailedCount == 0 && o.SuccessCount == 0);
                await ClearStatisticsAsync<ApiUserActionStatistic>(o => o.Count == 0);
            }

            await ClearStatisticsAsync<DailyAvgTimes>(o => o.ExternalApi == -1 && o.Interactions == -1 && o.InternalApi == -1 && o.Jobs == -1);

            if (payload.Type is LogType.InteractionCommand)
            {
                await ClearStatisticsAsync<InteractionUserActionStatistic>(o => o.Count == 0);
                await ClearStatisticsAsync<InteractionStatistic>(o => o.FailedCount == 0 && o.SuccessCount == 0);
            }

            if (payload.Type is LogType.JobCompleted)
                await ClearStatisticsAsync<JobInfo>(o => o.StartCount == 0);
        }

        await ClearStatisticsAsync<DatabaseStatistic>(o => o.RecordsCount == 0);
    }

    private async Task ClearStatisticsAsync<TStatisticEntity>(Expression<Func<TStatisticEntity, bool>> searchExpression) where TStatisticEntity : class
        => await StatisticsContext.Set<TStatisticEntity>().Where(searchExpression).ExecuteDeleteAsync();
}
