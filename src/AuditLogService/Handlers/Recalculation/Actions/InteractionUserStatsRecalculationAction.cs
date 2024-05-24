using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Models.Events.Recalculation;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Handlers.Recalculation.Actions;

public class InteractionUserStatsRecalculationAction : RecalculationActionBase
{
    public InteractionUserStatsRecalculationAction(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    public override bool CheckPreconditions(RecalculationPayload payload)
        => payload.Interaction is not null;

    public override async Task ProcessAsync(RecalculationPayload payload)
    {
        var action = $"{payload.Interaction!.Name} ({payload.Interaction.ModuleName}/{payload.Interaction.MethodName})";
        var stats = await GetOrCreateStatEntity<InteractionUserActionStatistic>(o => o.Action == action && o.UserId == payload.Interaction.UserId, action, payload.Interaction.UserId);

        stats.Count = await DbContext.InteractionCommands.AsNoTracking()
            .Where(o => o.LogItem.UserId == payload.Interaction.UserId)
            .LongCountAsync(o => o.Name == payload.Interaction.Name && o.ModuleName == payload.Interaction.ModuleName && o.MethodName == payload.Interaction.MethodName);
        await StatisticsContext.SaveChangesAsync();
    }
}
