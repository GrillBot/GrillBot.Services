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
        var currentStats = await GetOrCreateCurrentStatisticsAsync(result, apiGroup);

        currentStats.Count = await Context.ApiRequests.AsNoTracking()
            .LongCountAsync(o => o.Result == result && o.ApiGroupName == apiGroup);
        await StatisticsContext.SaveChangesAsync();
    }

    private async Task<ApiResultCountStatistic> GetOrCreateCurrentStatisticsAsync(string result, string apiGroup)
    {
        var stats = await StatisticsContext.ResultCountStatistic
            .FirstOrDefaultAsync(o => o.Result == result && o.ApiGroup == apiGroup);

        if (stats is null)
        {
            stats = new ApiResultCountStatistic
            {
                ApiGroup = apiGroup,
                Result = result
            };

            await StatisticsContext.AddAsync(stats);
        }

        return stats;
    }
}
