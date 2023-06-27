using System.Linq.Expressions;
using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Info.Dashboard;

namespace AuditLogService.Actions.Dashboard;

public class GetApiDashboardAction : DashboardListBaseAction<ApiRequest>
{
    public GetApiDashboardAction(AuditLogServiceContext context) : base(context)
    {
    }

    protected override Expression<Func<ApiRequest, DateTime>> CreateSorting()
        => entity => entity.EndAt;

    protected override Expression<Func<ApiRequest, DashboardInfoRow>> CreateProjection()
    {
        return entity => new DashboardInfoRow
        {
            Duration = (int)Math.Round((entity.EndAt - entity.StartAt).TotalMilliseconds),
            Name = $"{entity.Method} {entity.TemplatePath}",
            Result = entity.Result
        };
    }

    protected override Expression<Func<ApiRequest, bool>> CreateFilter()
    {
        var apiGroup = (string)Parameters[0]!;
        return entity => entity.ApiGroupName == apiGroup && entity.ActionName != "GetDashboardAsync" && entity.ControllerName != "DashboardController";
    }
}
