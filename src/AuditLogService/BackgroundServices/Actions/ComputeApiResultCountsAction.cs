using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.BackgroundServices.Actions;

public class ComputeApiResultCountsAction : PostProcessActionBase
{
    public ComputeApiResultCountsAction(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext) : base(context, statisticsContext)
    {
    }

    public override bool CanProcess(LogItem logItem)
        => logItem.Type is LogType.Api;

    public override async Task ProcessAsync(LogItem logItem)
    {
        var result = logItem.ApiRequest!.Result;
        var apiGroup = logItem.ApiRequest!.ApiGroupName;
        var stats = await GetOrCreateStatisticEntity<ApiResultCountStatistic>(o => o.Result == result && o.ApiGroup == apiGroup);

        stats.ApiGroup = apiGroup;
        stats.Result = result;
        stats.Count = await Context.ApiRequests.AsNoTracking()
            .LongCountAsync(o => o.Result == result && o.ApiGroupName == apiGroup);
        await StatisticsContext.SaveChangesAsync();
    }
}
