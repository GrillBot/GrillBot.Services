using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.BackgroundServices.Actions;

public class ComputeApiDateCountsAction : PostProcessActionBase
{
    public ComputeApiDateCountsAction(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext) : base(context, statisticsContext)
    {
    }

    public override bool CanProcess(LogItem logItem)
        => logItem.Type is LogType.Api;

    public override async Task ProcessAsync(LogItem logItem)
    {
        var date = DateOnly.FromDateTime(logItem.CreatedAt.Date);
        var apiGroup = logItem.ApiRequest!.ApiGroupName;
        var currentStats = await GetOrCreateCurrentStatisticsAsync(date, apiGroup);

        currentStats.Count = await Context.ApiRequests.AsNoTracking()
            .LongCountAsync(o => o.EndAt.Date == logItem.CreatedAt.Date && o.ApiGroupName == apiGroup);
        await StatisticsContext.SaveChangesAsync();
    }

    private async Task<ApiDateCountStatistic> GetOrCreateCurrentStatisticsAsync(DateOnly date, string apiGroup)
    {
        var stats = await StatisticsContext.DateCountStatistics
            .FirstOrDefaultAsync(o => o.Date == date && o.ApiGroup == apiGroup);

        if (stats is null)
        {
            stats = new ApiDateCountStatistic
            {
                ApiGroup = apiGroup,
                Date = date
            };

            await StatisticsContext.AddAsync(stats);
        }

        return stats;
    }
}
