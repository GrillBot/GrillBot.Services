using AuditLogService.Core.Entity.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Info;

public class GetJobsInfoAction : ApiAction
{
    private AuditLogStatisticsContext StatisticsContext { get; }

    public GetJobsInfoAction(AuditLogStatisticsContext statisticsContext, ICounterManager counterManager) : base(counterManager)
    {
        StatisticsContext = statisticsContext;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var query = StatisticsContext.JobInfos.AsNoTracking()
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

        using (CreateCounter("Database"))
        {
            var result = await query.ToListAsync();
            return ApiResult.Ok(result);
        }
    }
}
