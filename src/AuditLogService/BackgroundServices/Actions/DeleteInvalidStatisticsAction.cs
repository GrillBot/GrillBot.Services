using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.BackgroundServices.Actions;

public class DeleteInvalidStatisticsAction : PostProcessActionBase
{
    public DeleteInvalidStatisticsAction(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext) : base(context, statisticsContext)
    {
    }

    public override bool CanProcess(LogItem logItem)
        => logItem.Type is LogType.Api or LogType.InteractionCommand or LogType.JobCompleted;

    public override async Task ProcessAsync(LogItem logItem)
    {
        if (logItem.Type is LogType.Api)
        {
            var dateCountStatistics = await StatisticsContext.DateCountStatistics.Where(o => o.Count == 0).ToListAsync();
            if (dateCountStatistics.Count > 0)
                StatisticsContext.RemoveRange(dateCountStatistics);

            var resultCountStatistics = await StatisticsContext.ResultCountStatistic.Where(o => o.Count == 0).ToListAsync();
            if (resultCountStatistics.Count > 0)
                StatisticsContext.RemoveRange(resultCountStatistics);

            var requestStats = await StatisticsContext.RequestStats.Where(o => o.FailedCount == 0 && o.SuccessCount == 0).ToListAsync();
            if (requestStats.Count > 0)
                StatisticsContext.RemoveRange(requestStats);
        }

        var dailyAvgTimes = await StatisticsContext.DailyAvgTimes.Where(o => o.ExternalApi == -1 && o.Interactions == -1 && o.InternalApi == -1 && o.Jobs == -1).ToListAsync();
        if (dailyAvgTimes.Count > 0)
            StatisticsContext.RemoveRange(dailyAvgTimes);

        await StatisticsContext.SaveChangesAsync();
    }
}
