using AuditLogService.Core.Entity.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Info;

public class GetJobsInfoAction : ApiActionBase
{
    private AuditLogStatisticsContext StatisticsContext { get; }

    public GetJobsInfoAction(AuditLogStatisticsContext statisticsContext)
    {
        StatisticsContext = statisticsContext;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var result = await StatisticsContext.JobInfos.AsNoTracking()
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
            }).ToListAsync();

        return ApiResult.FromSuccess(result);
    }
}
