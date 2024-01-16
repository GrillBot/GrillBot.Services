using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Info.Dashboard;
using System.Linq.Expressions;

namespace AuditLogService.Actions.Dashboard;

public class GetMemberWarningDashboardAction : DashboardListBaseAction<MemberWarning>
{
    public GetMemberWarningDashboardAction(AuditLogServiceContext context) : base(context)
    {
    }

    protected override Expression<Func<MemberWarning, bool>>? CreateFilter() => null;

    protected override Expression<Func<MemberWarning, DashboardInfoRow>> CreateProjection()
    {
        return entity => new DashboardInfoRow
        {
            Name = entity.TargetId,
            Result = entity.LogItem.CreatedAt.ToString("o")
        };
    }

    protected override Expression<Func<MemberWarning, DateTime>> CreateSorting()
        => entity => entity.LogItem.CreatedAt;
}
