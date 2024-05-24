using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Events.Recalculation;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Handlers.Recalculation.Actions;

public class DailyAvgTimesRecalculationAction : RecalculationActionBase
{
    public DailyAvgTimesRecalculationAction(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override bool CheckPreconditions(RecalculationPayload payload)
        => payload.Api is not null || payload.Interaction is not null || payload.Job is not null;

    public override async Task ProcessAsync(RecalculationPayload payload)
    {
        var date = (payload.Api?.RequestDate ?? payload.Interaction?.EndDate ?? payload.Job?.JobDate)!.Value;
        var stats = await GetOrCreateStatEntity<DailyAvgTimes>(o => o.Date == date, date);

        if (payload.Type is LogType.Api && payload.Api is not null)
        {
            await ProcessApiTimesAsync(new[] { "V1", "V3" }, payload, stats, date);
            await ProcessApiTimesAsync(new[] { "V2" }, payload, stats, date);
        }

        if (payload.Type is LogType.InteractionCommand && payload.Interaction is not null)
            await ProcessInteractionsAsync(stats, date);

        if (payload.Type is LogType.JobCompleted && payload.Job is not null)
            await ProcessJobsAsync(stats, date);

        await StatisticsContext.SaveChangesAsync();
    }

    private async Task ProcessApiTimesAsync(string[] expectedApiGroups, RecalculationPayload payload, DailyAvgTimes stats, DateOnly requestDate)
    {
        if (!expectedApiGroups.Contains(payload.Api!.ApiGroupName))
            return;

        var avgTime = -1D;
        var query = DbContext.ApiRequests.AsNoTracking()
            .Where(o => o.RequestDate == requestDate);

        query = expectedApiGroups.Length == 1
            ? query.Where(o => o.ApiGroupName == expectedApiGroups[0])
            : query.Where(o => expectedApiGroups.Contains(o.ApiGroupName));

        if (await query.AnyAsync())
            avgTime = await query.AverageAsync(o => o.Duration);

        if (expectedApiGroups.Length == 1 && expectedApiGroups[0] == "V2")
            stats.ExternalApi = avgTime;
        else
            stats.InternalApi = avgTime;
    }

    private async Task ProcessInteractionsAsync(DailyAvgTimes stats, DateOnly interactionDate)
    {
        var query = DbContext.InteractionCommands.AsNoTracking()
            .Where(o => o.InteractionDate == interactionDate);

        stats.Interactions = await query.AnyAsync() ? await query.AverageAsync(o => o.Duration) : -1D;
    }

    private async Task ProcessJobsAsync(DailyAvgTimes stats, DateOnly jobDate)
    {
        var query = DbContext.JobExecutions.AsNoTracking()
            .Where(o => o.JobDate == jobDate);

        stats.Jobs = await query.AnyAsync() ? await query.AverageAsync(o => o.Duration) : -1D;
    }
}
