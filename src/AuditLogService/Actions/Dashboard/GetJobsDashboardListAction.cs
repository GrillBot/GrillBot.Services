using System.Linq.Expressions;
using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Info.Dashboard;

namespace AuditLogService.Actions.Dashboard;

public class GetJobsDashboardListAction : DashboardListBaseAction<JobExecution>
{
    public GetJobsDashboardListAction(AuditLogServiceContext context) : base(context)
    {
    }

    protected override Expression<Func<JobExecution, DateTime>> CreateSorting()
        => entity => entity.EndAt;

    protected override Expression<Func<JobExecution, DashboardInfoRow>> CreateProjection()
    {
        return entity => new DashboardInfoRow
        {
            Duration = (int)Math.Round((entity.EndAt - entity.StartAt).TotalMilliseconds),
            Name = entity.JobName,
            Success = !entity.WasError
        };
    }

    protected override Expression<Func<JobExecution, bool>>? CreateFilter() => null;
}
