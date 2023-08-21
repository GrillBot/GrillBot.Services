using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AuditLogService.BackgroundServices.Actions;

public class DeleteInvalidStatisticsAction : PostProcessActionBase
{
    public DeleteInvalidStatisticsAction(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext) : base(context, statisticsContext)
    {
    }

    public override bool CanProcess(LogItem logItem) => true;

    public override async Task ProcessAsync(LogItem logItem)
    {
        if (logItem.Type is LogType.Api or LogType.InteractionCommand or LogType.JobCompleted)
        {
            if (logItem.Type is LogType.Api)
            {
                await ClearStatisticsAsync<ApiDateCountStatistic>(o => o.Count == 0);
                await ClearStatisticsAsync<ApiResultCountStatistic>(o => o.Count == 0);
                await ClearStatisticsAsync<ApiRequestStat>(o => o.FailedCount == 0 && o.SuccessCount == 0);
                await ClearStatisticsAsync<ApiUserActionStatistic>(o => o.Count == 0);
            }

            await ClearStatisticsAsync<DailyAvgTimes>(o => o.ExternalApi == -1 && o.Interactions == -1 && o.InternalApi == -1 && o.Jobs == -1);

            if (logItem.Type is LogType.InteractionCommand)
            {
                await ClearStatisticsAsync<InteractionUserActionStatistic>(o => o.Count == 0);
                await ClearStatisticsAsync<InteractionStatistic>(o => o.FailedCount == 0 && o.SuccessCount == 0);
            }

            if (logItem.Type is LogType.JobCompleted)
                await ClearStatisticsAsync<JobInfo>(o => o.StartCount == 0);
        }

        await ClearStatisticsAsync<AuditLogTypeStatistic>(o => o.Count == 0);
        await ClearStatisticsAsync<AuditLogDateStatistic>(o => o.Count == 0);

        if (logItem.Files.Count > 0)
            await ClearStatisticsAsync<FileExtensionStatistic>(o => o.Count == 0);

        await StatisticsContext.SaveChangesAsync();
    }

    private async Task ClearStatisticsAsync<TStatisticEntity>(Expression<Func<TStatisticEntity, bool>> searchExpression) where TStatisticEntity : class
    {
        var statistics = await StatisticsContext.Set<TStatisticEntity>().Where(searchExpression).ToListAsync();
        if (statistics.Count > 0)
            StatisticsContext.RemoveRange(statistics);
    }
}
