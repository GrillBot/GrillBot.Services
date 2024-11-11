using System.Linq.Expressions;
using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Info.Dashboard;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Dashboard;

public abstract class DashboardListBaseAction<TEntity> : ApiAction<AuditLogServiceContext> where TEntity : ChildEntityBase
{
    protected DashboardListBaseAction(AuditLogServiceContext context, ICounterManager counterManager) : base(counterManager, context)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var query = DbContext.Set<TEntity>()
            .OrderByDescending(CreateSorting())
            .Where(o => !o.LogItem.IsDeleted)
            .AsNoTracking();
        var filter = CreateFilter();
        if (filter is not null)
            query = query.Where(filter);

        var mappedQuery = query
            .Select(CreateProjection())
            .Take(50);

        var result = await ContextHelper.ReadEntitiesAsync(mappedQuery);
        return ApiResult.Ok(result);
    }

    protected abstract Expression<Func<TEntity, DateTime>> CreateSorting();
    protected abstract Expression<Func<TEntity, DashboardInfoRow>> CreateProjection();
    protected abstract Expression<Func<TEntity, bool>>? CreateFilter();
}
