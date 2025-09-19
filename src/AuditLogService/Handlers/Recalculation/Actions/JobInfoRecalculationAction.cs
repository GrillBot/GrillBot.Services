using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Events.Recalculation;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Handlers.Recalculation.Actions;

public class JobInfoRecalculationAction(IServiceProvider serviceProvider) : RecalculationActionBase(serviceProvider)
{
    public override bool CheckPreconditions(RecalculationPayload payload)
        => !string.IsNullOrEmpty(payload.Job?.JobName);

    public override async Task ProcessAsync(RecalculationPayload payload)
    {
        var stats = await GetOrCreateStatEntity<JobInfo>(o => o.Name == payload.Job!.JobName, payload.Job!.JobName);
        var dataQuery = DbContext.JobExecutions.AsNoTracking()
            .Where(o => o.JobName == payload.Job.JobName);

        if (await dataQuery.AnyAsync())
        {
            stats.AvgTime = (int)Math.Round(await dataQuery.AverageAsync(o => o.Duration));
            stats.FailedCount = await dataQuery.CountAsync(o => o.WasError);
            stats.MaxTime = (int)await dataQuery.MaxAsync(o => o.Duration);
            stats.MinTime = (int)await dataQuery.MinAsync(o => o.Duration);
            stats.StartCount = await dataQuery.CountAsync();
            stats.TotalDuration = (int)await dataQuery.SumAsync(o => o.Duration);
            stats.LastStartAt = await dataQuery.MaxAsync(o => o.StartAt);

            stats.LastRunDuration = await dataQuery
                .OrderByDescending(o => o.EndAt)
                .Select(o => (int)o.Duration)
                .FirstOrDefaultAsync();
        }
        else
        {
            stats.StartCount = 0;
        }

        await StatisticsContext.SaveChangesAsync();
    }
}
