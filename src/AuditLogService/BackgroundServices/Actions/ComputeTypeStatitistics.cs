using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.BackgroundServices.Actions;

public class ComputeTypeStatitistics : PostProcessActionBase
{
    public ComputeTypeStatitistics(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext) : base(context, statisticsContext)
    {
    }

    public override bool CanProcess(LogItem logItem) => true;

    public override async Task ProcessAsync(LogItem logItem)
    {
        var stats = await GetOrCreateStatisticEntity<AuditLogTypeStatistic>(o => o.Type == logItem.Type, logItem.Type);

        stats.Type = logItem.Type;
        stats.Count = await Context.LogItems.AsNoTracking()
            .LongCountAsync(o => o.Type == logItem.Type);
        await StatisticsContext.SaveChangesAsync();
    }
}
