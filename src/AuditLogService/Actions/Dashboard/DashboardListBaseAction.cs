using System.Linq.Expressions;
using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Info.Dashboard;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Dashboard;

public abstract class DashboardListBaseAction<TEntity> : ApiAction where TEntity : ChildEntityBase
{
    private AuditLogServiceContext Context { get; }

    protected DashboardListBaseAction(AuditLogServiceContext context, ICounterManager counterManager) : base(counterManager)
    {
        Context = context;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var query = Context.Set<TEntity>()
            .OrderByDescending(CreateSorting())
            .AsNoTracking();
        var filter = CreateFilter();
        if (filter is not null)
            query = query.Where(filter);

        var mappedQuery = query
            .Select(CreateProjection())
            .Take(10);

        using (CreateCounter("Database"))
        {
            var result = await mappedQuery.ToListAsync();
            return ApiResult.Ok(result);
        }
    }

    protected abstract Expression<Func<TEntity, DateTime>> CreateSorting();
    protected abstract Expression<Func<TEntity, DashboardInfoRow>> CreateProjection();
    protected abstract Expression<Func<TEntity, bool>>? CreateFilter();
}
