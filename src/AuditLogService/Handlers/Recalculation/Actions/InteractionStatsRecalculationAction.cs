using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Events.Recalculation;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Handlers.Recalculation.Actions;

public class InteractionStatsRecalculationAction : RecalculationActionBase
{
    public InteractionStatsRecalculationAction(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override bool CheckPreconditions(RecalculationPayload payload)
        => payload.Interaction is not null;

    public override async Task ProcessAsync(RecalculationPayload payload)
    {
        var interaction = payload.Interaction!;
        var action = $"{interaction.Name} ({interaction.ModuleName}/{interaction.MethodName})";
        var stats = await GetOrCreateStatEntity<InteractionStatistic>(o => o.Action == action, action);

        var dataQuery = DbContext.InteractionCommands.AsNoTracking()
            .Where(o => o.Name == interaction.Name && o.ModuleName == interaction.ModuleName && o.MethodName == interaction.MethodName);

        if (await dataQuery.AnyAsync())
        {
            stats.LastRun = await dataQuery.MaxAsync(o => o.EndAt);
            stats.FailedCount = await dataQuery.LongCountAsync(o => !o.IsSuccess);
            stats.MaxDuration = await dataQuery.MaxAsync(o => o.Duration);
            stats.MinDuration = await dataQuery.MinAsync(o => o.Duration);
            stats.SuccessCount = await dataQuery.LongCountAsync(o => o.IsSuccess);
            stats.TotalDuration = await dataQuery.SumAsync(o => o.Duration);
            stats.LastRunDuration = await dataQuery.OrderByDescending(o => o.EndAt).Select(o => o.Duration).FirstAsync();
        }
        else
        {
            stats.FailedCount = 0;
            stats.SuccessCount = 0;
        }

        await StatisticsContext.SaveChangesAsync();
    }
}
