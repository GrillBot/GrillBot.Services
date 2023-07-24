using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.BackgroundServices.Actions;

public class ComputeDateStatisticsAction : PostProcessActionBase
{
    public ComputeDateStatisticsAction(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext) : base(context, statisticsContext)
    {
    }

    public override bool CanProcess(LogItem logItem) => true;

    public override async Task ProcessAsync(LogItem logItem)
    {
        var date = DateOnly.FromDateTime(logItem.CreatedAt);
        var stats = await GetOrCreateStatisticEntity<AuditLogDateStatistic>(o => o.Date == date, date);

        stats.Date = date;
        stats.Count = await Context.LogItems.AsNoTracking()
            .LongCountAsync(o => o.CreatedAt.Date == logItem.CreatedAt.Date && !o.IsDeleted);
        await StatisticsContext.SaveChangesAsync();
    }
}
