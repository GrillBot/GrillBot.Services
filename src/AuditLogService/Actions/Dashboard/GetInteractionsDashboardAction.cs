using System.Linq.Expressions;
using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Info.Dashboard;
using GrillBot.Core.Managers.Performance;

namespace AuditLogService.Actions.Dashboard;

public class GetInteractionsDashboardAction(
    AuditLogServiceContext context,
    ICounterManager counterManager
) : DashboardListBaseAction<InteractionCommand>(context, counterManager)
{
    protected override Expression<Func<InteractionCommand, DateTime>> CreateSorting()
        => entity => entity.EndAt;

    protected override Expression<Func<InteractionCommand, DashboardInfoRow>> CreateProjection()
    {
        return entity => new DashboardInfoRow
        {
            Duration = entity.Duration,
            Name = $"{entity.Name} ({entity.ModuleName}/{entity.MethodName})",
            Success = entity.IsSuccess
        };
    }

    protected override Expression<Func<InteractionCommand, bool>>? CreateFilter() => null;
}
