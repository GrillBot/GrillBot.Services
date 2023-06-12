using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetUserCommandStatisticsAction : ApiActionBase
{
    private AuditLogServiceContext Context { get; }

    public GetUserCommandStatisticsAction(AuditLogServiceContext context)
    {
        Context = context;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var data = await Context.InteractionCommands.AsNoTracking()
            .GroupBy(o => new { o.LogItem.UserId, o.Name, o.ModuleName, o.MethodName })
            .Select(o => new { o.Key.UserId, o.Key.ModuleName, o.Key.MethodName, o.Key.Name, Count = o.Count() })
            .ToListAsync();

        var result = data.Select(o => new UserActionCountItem
        {
            Action = $"{o.Name} ({o.ModuleName}/{o.MethodName})",
            Count = o.Count,
            UserId = o.UserId!
        }).OrderBy(o => o.Action).ToList();

        return new ApiResult(StatusCodes.Status200OK, result);
    }
}
