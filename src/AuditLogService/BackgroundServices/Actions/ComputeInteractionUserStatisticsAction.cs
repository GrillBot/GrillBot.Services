using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.BackgroundServices.Actions;

public class ComputeInteractionUserStatisticsAction : PostProcessActionBase
{
    public ComputeInteractionUserStatisticsAction(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext) : base(context, statisticsContext)
    {
    }

    public override bool CanProcess(LogItem logItem)
        => logItem.Type is LogType.InteractionCommand;

    public override async Task ProcessAsync(LogItem logItem)
    {
        var interaction = logItem.InteractionCommand!;
        var action = $"{interaction.Name} ({interaction.ModuleName}/{interaction.MethodName})";
        var userId = logItem.UserId!;
        var stats = await GetOrCreateStatisticEntity<InteractionUserActionStatistic>(o => o.Action == action && o.UserId == userId, action, userId);

        stats.Count = await Context.InteractionCommands.AsNoTracking()
            .Where(o => !Context.LogItems.Any(x => x.IsDeleted && o.LogItemId == x.Id))
            .CountAsync(o => o.LogItem.UserId == userId && o.Name == interaction.Name && o.ModuleName == interaction.ModuleName && o.MethodName == interaction.MethodName);
        await StatisticsContext.SaveChangesAsync();
    }
}
