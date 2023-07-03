using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Info;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Info;

public class GetJobsInfoAction : ApiActionBase
{
    private AuditLogServiceContext Context { get; }

    public GetJobsInfoAction(AuditLogServiceContext context)
    {
        Context = context;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var result = await Context.JobExecutions.AsNoTracking()
            .GroupBy(o => o.JobName)
            .Select(o => new JobInfo
            {
                Name = o.Key,
                AvgTime = (int)Math.Round(o.Average(x => (x.EndAt - x.StartAt).TotalMilliseconds)),
                FailedCount = o.Count(x => x.WasError),
                MaxTime = (int)Math.Round(o.Max(x => (x.EndAt - x.StartAt).TotalMilliseconds)),
                MinTime = (int)Math.Round(o.Min(x => (x.EndAt - x.StartAt).TotalMilliseconds)),
                StartCount = o.Count(),
                TotalDuration = (int)Math.Round(o.Sum(x => (x.EndAt - x.StartAt).TotalMilliseconds)),
                LastStartAt = o.Max(x => x.StartAt)
            })
            .ToListAsync();

        foreach (var job in result)
        {
            var lastItem = await Context.JobExecutions.AsNoTracking()
                .Where(o => o.JobName == job.Name)
                .OrderByDescending(o => o.StartAt)
                .Select(o => new { o.EndAt, o.StartAt })
                .FirstOrDefaultAsync();
            job.LastRunDuration = lastItem is null ? null : (int)Math.Round((lastItem.EndAt - lastItem.StartAt).TotalMilliseconds);
        }

        return ApiResult.FromSuccess(result);
    }
}
