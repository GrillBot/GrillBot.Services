﻿using System.Linq.Expressions;
using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Info.Dashboard;
using GrillBot.Core.Managers.Performance;

namespace AuditLogService.Actions.Dashboard;

public class GetJobsDashboardListAction : DashboardListBaseAction<JobExecution>
{
    public GetJobsDashboardListAction(AuditLogServiceContext context, ICounterManager counterManager) : base(context, counterManager)
    {
    }

    protected override Expression<Func<JobExecution, DateTime>> CreateSorting()
        => entity => entity.EndAt;

    protected override Expression<Func<JobExecution, DashboardInfoRow>> CreateProjection()
    {
        return entity => new DashboardInfoRow
        {
            Duration = (int)entity.Duration,
            Name = entity.JobName,
            Success = !entity.WasError
        };
    }

    protected override Expression<Func<JobExecution, bool>>? CreateFilter() => null;
}
