using System.Linq.Expressions;
using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Info.Dashboard;

namespace AuditLogService.Actions.Dashboard;

public class GetInteractionsDashboardAction : DashboardListBaseAction<InteractionCommand>
{
    public GetInteractionsDashboardAction(AuditLogServiceContext context) : base(context)
    {
    }

    protected override Expression<Func<InteractionCommand, DateTime>> CreateSorting()
        => entity => entity.LogItem.CreatedAt;

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
