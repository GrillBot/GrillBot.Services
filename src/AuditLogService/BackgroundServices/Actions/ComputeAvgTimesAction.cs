using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.BackgroundServices.Actions;

public class ComputeAvgTimesAction : PostProcessActionBase
{
    public ComputeAvgTimesAction(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext) : base(context, statisticsContext)
    {
    }

    public override bool CanProcess(LogItem logItem)
        => logItem.Type is LogType.Api or LogType.InteractionCommand or LogType.JobCompleted;

    public override async Task ProcessAsync(LogItem logItem)
    {
        var date = logItem.LogDate;
        var stats = await GetOrCreateStatisticEntity<DailyAvgTimes>(o => o.Date == date, date);

        await ProcessApiTimesAsync("V1", logItem, stats, date);
        await ProcessApiTimesAsync("V2", logItem, stats, date);
        await ProcessInteractionsAsync(logItem, stats, date);
        await ProcessJobsAsync(logItem, stats, date);
        await StatisticsContext.SaveChangesAsync();
    }

    private async Task ProcessApiTimesAsync(string expectedApiGroup, LogItem logItem, DailyAvgTimes stats, DateOnly requestDate)
    {
        if (logItem.Type is not LogType.Api || logItem.ApiRequest!.ApiGroupName != expectedApiGroup) return;

        var avgTime = -1D;
        var query = Context.ApiRequests.AsNoTracking()
            .Where(o => !Context.LogItems.Any(x => x.IsDeleted && o.LogItemId == x.Id))
            .Where(o => o.ApiGroupName == expectedApiGroup && o.RequestDate == requestDate);
        if (await query.AnyAsync())
            avgTime = await query.AverageAsync(o => o.Duration);

        if (expectedApiGroup == "V2")
            stats.ExternalApi = avgTime;
        else
            stats.InternalApi = avgTime;
    }

    private async Task ProcessInteractionsAsync(LogItem logItem, DailyAvgTimes stats, DateOnly interactionDate)
    {
        if (logItem.Type is not LogType.InteractionCommand) return;

        var avgTime = -1D;
        var query = Context.InteractionCommands.AsNoTracking()
            .Where(o => !Context.LogItems.Any(x => x.IsDeleted && o.LogItemId == x.Id) && o.InteractionDate == interactionDate);
        if (await query.AnyAsync())
            avgTime = await query.AverageAsync(o => o.Duration);

        stats.Interactions = avgTime;
    }

    private async Task ProcessJobsAsync(LogItem logItem, DailyAvgTimes stats, DateOnly jobDate)
    {
        if (logItem.Type is not LogType.JobCompleted) return;

        var avgTime = -1D;
        var query = Context.JobExecutions.AsNoTracking()
            .Where(o => !Context.LogItems.Any(x => x.IsDeleted && o.LogItemId == x.Id) && o.JobDate == jobDate);
        if (await query.AnyAsync())
            avgTime = await query.AverageAsync(o => o.Duration);

        stats.Jobs = avgTime;
    }
}
