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
        var currentStats = await GetOrCreateStatisticAsync(action, userId);

        currentStats.Count = await Context.InteractionCommands.AsNoTracking()
            .CountAsync(o => o.LogItem.UserId == userId && o.Name == interaction.Name && o.ModuleName == interaction.ModuleName && o.MethodName == interaction.MethodName);
        await StatisticsContext.SaveChangesAsync();
    }

    private async Task<InteractionUserActionStatistic> GetOrCreateStatisticAsync(string action, string userId)
    {
        var stats = await StatisticsContext.InteractionUserActionStatistics
            .FirstOrDefaultAsync(o => o.Action == action && o.UserId == userId);

        if (stats is null)
        {
            stats = new InteractionUserActionStatistic
            {
                UserId = userId,
                Action = action
            };

            await StatisticsContext.AddAsync(stats);
        }

        return stats;
    }
}
