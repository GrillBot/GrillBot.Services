using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.BackgroundServices.Actions;

public class ComputeInteractionStatisticsAction : PostProcessActionBase
{
    public ComputeInteractionStatisticsAction(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext) : base(context, statisticsContext)
    {
    }

    public override bool CanProcess(LogItem logItem)
        => logItem.Type is LogType.InteractionCommand;

    public override async Task ProcessAsync(LogItem logItem)
    {
        var interaction = logItem.InteractionCommand!;
        var action = $"{interaction.Name} ({interaction.ModuleName}/{interaction.MethodName})";
        var stats = await GetOrCreateStatisticEntity<InteractionStatistic>(o => o.Action == action, action);
        var data = await Context.InteractionCommands.AsNoTracking()
            .Where(o => (o.LogItem.Flags & LogItemFlag.Deleted) == 0 && o.Name == interaction.Name && o.ModuleName == interaction.ModuleName && o.MethodName == interaction.MethodName)
            .GroupBy(_ => 1)
            .Select(o => new
            {
                LastRun = o.Max(x => x.LogItem.CreatedAt),
                FailedCount = o.LongCount(x => !x.IsSuccess),
                MaxDuration = o.Max(x => x.Duration),
                MinDuration = o.Min(x => x.Duration),
                SuccessCount = o.LongCount(x => x.IsSuccess),
                TotalDuration = o.Sum(x => x.Duration),
                LastRunDuration = o.OrderByDescending(x => x.LogItem.CreatedAt).First().Duration
            }).FirstOrDefaultAsync();

        stats.Action = action;
        if (data is null)
        {
            stats.FailedCount = 0;
            stats.SuccessCount = 0;
        }
        else
        {
            stats.LastRun = data.LastRun;
            stats.FailedCount = data.FailedCount;
            stats.MaxDuration = data.MaxDuration;
            stats.MinDuration = data.MinDuration;
            stats.SuccessCount = data.SuccessCount;
            stats.TotalDuration = data.TotalDuration;
            stats.LastRunDuration = data.LastRunDuration;
        }

        await StatisticsContext.SaveChangesAsync();
    }
}
