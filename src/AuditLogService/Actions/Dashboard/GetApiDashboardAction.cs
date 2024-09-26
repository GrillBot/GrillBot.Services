using System.Linq.Expressions;
using System.Text.RegularExpressions;
using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Info.Dashboard;
using GrillBot.Core.Managers.Performance;

#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
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
        var apiGroup = GetParameter<string>(0);

        var internalApis = new[] { "V1", "V3" };
        var ignoredControllers = new[] { "DashboardController", "LookupController" };

        if (apiGroup == "V2")
        {
            return entity => entity.ApiGroupName == "V2";
        }
        else
        {

            return entity => internalApis.Contains(entity.ApiGroupName) &&
                entity.ActionName != "GetDashboardAsync" &&
                !ignoredControllers.Contains(entity.ControllerName) &&
                !Regex.IsMatch(entity.TemplatePath, ".+/dashboard/?.*");
        }
    }
}
