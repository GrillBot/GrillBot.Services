using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetInteractionStatisticsListAction : ApiActionBase
{
    private AuditLogServiceContext Context { get; }

    public GetInteractionStatisticsListAction(AuditLogServiceContext context)
    {
        Context = context;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var result = await Context.InteractionCommands.AsNoTracking()
            .GroupBy(o => new { o.Name, o.ModuleName, o.MethodName })
            .Select(o => new StatisticItem
            {
                Key = $"{o.Key.Name} ({o.Key.ModuleName}/{o.Key.MethodName})",
                Last = o.Max(x => x.LogItem.CreatedAt),
                FailedCount = o.Count(x => !x.IsSuccess),
                MaxDuration = o.Max(x => x.Duration),
                MinDuration = o.Min(x => x.Duration),
                SuccessCount = o.Count(x => x.IsSuccess),
                TotalDuration = o.Sum(x => x.Duration),
                LastRunDuration = o.OrderByDescending(x => x.LogItem.CreatedAt).First().Duration
            })
            .ToListAsync();

        result = result
            .OrderByDescending(o => o.AvgDuration)
            .ThenByDescending(o => o.SuccessCount + o.FailedCount)
            .ThenBy(o => o.Key)
            .ToList();

        return new ApiResult(StatusCodes.Status200OK, result);
    }
}
