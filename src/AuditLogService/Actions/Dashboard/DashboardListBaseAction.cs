using System.Linq.Expressions;
using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Info.Dashboard;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Dashboard;

public abstract class DashboardListBaseAction<TEntity> : ApiActionBase where TEntity : ChildEntityBase
{
    private AuditLogServiceContext Context { get; }

    protected DashboardListBaseAction(AuditLogServiceContext context)
    {
        Context = context;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var query = Context.Set<TEntity>()
            .OrderByDescending(CreateSorting())
            .AsQueryable();
        var filter = CreateFilter();
        if (filter is not null)
            query = query.Where(filter);

        var mappedQuery = query
            .Select(CreateProjection())
            .Take(10);

        var result = await mappedQuery.ToListAsync();
        return new ApiResult(StatusCodes.Status200OK, result);
    }

    protected abstract Expression<Func<TEntity, DateTime>> CreateSorting();
    protected abstract Expression<Func<TEntity, DashboardInfoRow>> CreateProjection();
    protected abstract Expression<Func<TEntity, bool>>? CreateFilter();
}
