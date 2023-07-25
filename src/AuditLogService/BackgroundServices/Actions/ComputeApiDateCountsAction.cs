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
        var stats = await GetOrCreateStatisticEntity<ApiDateCountStatistic>(o => o.Date == date && o.ApiGroup == apiGroup, date, apiGroup);

        stats.ApiGroup = apiGroup;
        stats.Date = date;
        stats.Count = await Context.ApiRequests.AsNoTracking()
            .Where(o => !Context.LogItems.Any(x => x.IsDeleted && o.LogItemId == x.Id))
            .LongCountAsync(o => o.RequestDate == date && o.ApiGroupName == apiGroup);
        await StatisticsContext.SaveChangesAsync();
    }
}
