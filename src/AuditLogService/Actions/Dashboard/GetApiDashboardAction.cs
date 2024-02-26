using System.Linq.Expressions;
using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Info.Dashboard;
using GrillBot.Core.Managers.Performance;

namespace AuditLogService.Actions.Dashboard;

public class GetApiDashboardAction : DashboardListBaseAction<ApiRequest>
{
    public GetApiDashboardAction(AuditLogServiceContext context, ICounterManager counterManager) : base(context, counterManager)
    {
    }

    protected override Expression<Func<ApiRequest, DateTime>> CreateSorting()
        => entity => entity.EndAt;

    protected override Expression<Func<ApiRequest, DashboardInfoRow>> CreateProjection()
    {
        return entity => new DashboardInfoRow
        {
            Duration = (int)entity.Duration,
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
