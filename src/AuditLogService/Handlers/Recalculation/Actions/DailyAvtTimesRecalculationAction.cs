using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Events.Recalculation;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Handlers.Recalculation.Actions;

public class DailyAvtTimesRecalculationAction : RecalculationActionBase
{
    public DailyAvtTimesRecalculationAction(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override async Task ProcessAsync(RecalculationPayload payload)
    {
        var date = (payload.Api?.RequestDate ?? payload.Interaction?.EndDate ?? payload.Job?.JobDate)!.Value;
        var stats = await GetOrCreateStatEntity<DailyAvgTimes>(o => o.Date == date, date);

        if (payload.Type is LogType.Api)
        {
            await ProcessApiTimesAsync("V1", payload, stats, date);
            await ProcessApiTimesAsync("V2", payload, stats, date);
            await ProcessApiTimesAsync("V3", payload, stats, date);
        }

        if (payload.Type is LogType.InteractionCommand)
            await ProcessInteractionsAsync(stats, date);

        if (payload.Type is LogType.JobCompleted)
            await ProcessJobsAsync(stats, date);

        await StatisticsContext.SaveChangesAsync();
    }

    private async Task ProcessApiTimesAsync(string expectedApiGroup, RecalculationPayload payload, DailyAvgTimes stats, DateOnly requestDate)
    {
        if (payload.Api!.ApiGroupName != expectedApiGroup)
            return;

        var avgTime = -1D;
        var query = DbContext.ApiRequests.AsNoTracking()
            .Where(o => o.ApiGroupName == expectedApiGroup && o.RequestDate == requestDate);
        if (await query.AnyAsync())
            avgTime = await query.AverageAsync(o => o.Duration);

        if (expectedApiGroup == "V2")
            stats.ExternalApi = avgTime;
        else
            stats.InternalApi = avgTime;
    }

    private async Task ProcessInteractionsAsync(DailyAvgTimes stats, DateOnly interactionDate)
    {
        var avgTime = -1D;
        var query = DbContext.InteractionCommands.AsNoTracking()
            .Where(o => o.InteractionDate == interactionDate);
        if (await query.AnyAsync())
            avgTime = await query.AverageAsync(o => o.Duration);

        stats.Interactions = avgTime;
    }

    private async Task ProcessJobsAsync(DailyAvgTimes stats, DateOnly jobDate)
    {
        var avgTime = -1D;
        var query = DbContext.JobExecutions.AsNoTracking()
            .Where(o => o.JobDate == jobDate);
        if (await query.AnyAsync())
            avgTime = await query.AverageAsync(o => o.Duration);

        stats.Jobs = avgTime;
    }
}
