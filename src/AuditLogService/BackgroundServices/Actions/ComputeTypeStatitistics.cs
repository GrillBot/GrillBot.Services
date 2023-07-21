using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Enums;
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
        var currentStats = await GetOrCreateCurrentStatsAsync(logItem.Type);

        currentStats.Count = await Context.LogItems.AsNoTracking()
            .LongCountAsync(o => o.Type == logItem.Type);
        await StatisticsContext.SaveChangesAsync();
    }

    private async Task<AuditLogTypeStatistic> GetOrCreateCurrentStatsAsync(LogType type)
    {
        var stats = await StatisticsContext.TypeStatistics
            .FirstOrDefaultAsync(o => o.Type == type);

        if (stats is null)
        {
            stats = new AuditLogTypeStatistic
            {
                Type = type
            };

            await StatisticsContext.AddAsync(stats);
        }

        return stats;
    }
}
