using AuditLogService.Core.Entity.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Info;

public class GetJobsInfoAction(
    AuditLogStatisticsContext statisticsContext,
    ICounterManager counterManager
) : ApiAction<AuditLogStatisticsContext>(counterManager, statisticsContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var query = DbContext.JobInfos.AsNoTracking()
            .Select(o => new Models.Response.Info.JobInfo
            {
                AvgTime = o.AvgTime,
                FailedCount = o.FailedCount,
                LastRunDuration = o.LastRunDuration,
                LastStartAt = o.LastStartAt,
                MaxTime = o.MaxTime,
                MinTime = o.MinTime,
                Name = o.Name,
                StartCount = o.StartCount,
                TotalDuration = o.TotalDuration
            });

        var result = await ContextHelper.ReadEntitiesAsync(query);
        return ApiResult.Ok(result);
    }
}
