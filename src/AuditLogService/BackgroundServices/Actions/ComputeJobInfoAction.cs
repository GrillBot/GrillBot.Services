using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.BackgroundServices.Actions;

public class ComputeJobInfoAction : PostProcessActionBase
{
    public ComputeJobInfoAction(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext) : base(context, statisticsContext)
    {
    }

    public override bool CanProcess(LogItem logItem)
        => logItem.Type is LogType.JobCompleted;

    public override async Task ProcessAsync(LogItem logItem)
    {
        var job = logItem.Job!;
        var stats = await GetOrCreateStatisticEntity<JobInfo>(o => o.Name == job.JobName, job.JobName);
        var baseQuery = Context.JobExecutions.AsNoTracking()
            .Where(o => o.JobName == job.JobName && !Context.LogItems.Any(x => o.LogItemId == x.Id && x.IsDeleted));

        var data = await baseQuery
            .GroupBy(_ => 1)
            .Select(g => new
            {
                AvgTime = (int)Math.Round(g.Average(x => (x.EndAt - x.StartAt).TotalMilliseconds)),
                FailedCount = g.Count(x => x.WasError),
                MaxTime = (int)Math.Round(g.Max(x => (x.EndAt - x.StartAt).TotalMilliseconds)),
                MinTime = (int)Math.Round(g.Min(x => (x.EndAt - x.StartAt).TotalMilliseconds)),
                StartCount = g.Count(),
                TotalDuration = (int)Math.Round(g.Sum(x => (x.EndAt - x.StartAt).TotalMilliseconds)),
                LastStartAt = g.Max(x => x.StartAt)
            }).FirstOrDefaultAsync();

        if (data is null)
        {
            stats.StartCount = 0;
        }
        else
        {
            stats.AvgTime = data.AvgTime;
            stats.FailedCount = data.FailedCount;
            stats.LastStartAt = data.LastStartAt;
            stats.MaxTime = data.MaxTime;
            stats.MinTime = data.MinTime;
            stats.StartCount = data.StartCount;
            stats.TotalDuration = data.TotalDuration;

            stats.LastRunDuration = await baseQuery
                .OrderByDescending(o => o.StartAt)
                .Select(o => (int)Math.Round((o.EndAt - o.StartAt).TotalMilliseconds))
                .FirstOrDefaultAsync();
        }

        await StatisticsContext.SaveChangesAsync();
    }
}
