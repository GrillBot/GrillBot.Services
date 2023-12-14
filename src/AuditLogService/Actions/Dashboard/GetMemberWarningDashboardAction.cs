using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Info.Dashboard;
using System.Linq.Expressions;

namespace AuditLogService.Actions.Dashboard;

public class GetMemberWarningDashboardAction : DashboardListBaseAction<Core.Entity.MemberWarning>
{
    public GetMemberWarningDashboardAction(AuditLogServiceContext context) : base(context)
    {
    }

    protected override Expression<Func<Core.Entity.MemberWarning, bool>>? CreateFilter() => null;

    protected override Expression<Func<Core.Entity.MemberWarning, DashboardInfoRow>> CreateProjection()
    {
        return entity => new DashboardInfoRow
        {
            Name = entity.TargetId
        };
    }

    protected override Expression<Func<Core.Entity.MemberWarning, DateTime>> CreateSorting()
        => entity => entity.LogItem.CreatedAt;
}
