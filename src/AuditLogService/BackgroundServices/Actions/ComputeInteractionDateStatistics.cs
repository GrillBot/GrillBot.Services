using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.BackgroundServices.Actions;

public class ComputeInteractionDateStatistics : PostProcessActionBase
{
    public ComputeInteractionDateStatistics(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext) : base(context, statisticsContext)
    {
    }

    public override bool CanProcess(LogItem logItem)
        => logItem.Type is LogType.InteractionCommand;

    public override async Task ProcessAsync(LogItem logItem)
    {
        var date = logItem.LogDate;
        var stats = await GetOrCreateStatisticEntity<InteractionDateCountStatistic>(o => o.Date == date, date);

        stats.Count = await Context.LogItems.AsNoTracking()
            .CountAsync(o => !o.IsDeleted && o.LogDate == date && o.Type == LogType.InteractionCommand);
        await StatisticsContext.SaveChangesAsync();
    }
}
