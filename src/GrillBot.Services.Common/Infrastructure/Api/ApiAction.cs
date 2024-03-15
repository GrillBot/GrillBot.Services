using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models.Pagination;
using Microsoft.EntityFrameworkCore;

namespace GrillBot.Services.Common.Infrastructure.Api;

public abstract class ApiAction<TDbContext> : ApiAction where TDbContext : DbContext
{
    protected TDbContext DbContext { get; }

    protected ApiAction(ICounterManager counterManager, TDbContext dbContext) : base(counterManager)
    {
        DbContext = dbContext;
    }

    protected async Task<List<TEntity>> ReadEntities<TEntity>(IQueryable<TEntity> query) where TEntity : class
    {
        using (CreateCounter("Database"))
            return await query.ToListAsync();
    }

    protected async Task<PaginatedResponse<TEntity>> ReadPaginatedEntities<TEntity>(IQueryable<TEntity> query, PaginatedParams parameters) where TEntity : class
    {
        using (CreateCounter("Database"))
            return await PaginatedResponse<TEntity>.CreateWithEntityAsync(query, parameters);
    }

    protected async Task<TEntity?> ReadFirstOrDefaultEntity<TEntity>(IQueryable<TEntity> query) where TEntity : class
    {
        using (CreateCounter("Database"))
            return await query.FirstOrDefaultAsync();
    }

    protected async Task<TEntity> ReadFirstEntityAsync<TEntity>(IQueryable<TEntity> query) where TEntity : class
    {
        using (CreateCounter("Database"))
            return await query.FirstAsync();
    }

    protected async Task<int> SaveChangesAsync()
    {
        using (CreateCounter("Database"))
            return await DbContext.SaveChangesAsync();
    }

    protected async Task<bool> IsAnyAsync<TEntity>(IQueryable<TEntity> query) where TEntity : class
    {
        using (CreateCounter("Database"))
            return await query.AnyAsync();
    }
}

public abstract class ApiAction : ApiActionBase
{
    private ICounterManager CounterManager { get; }

    protected ApiAction(ICounterManager counterManager)
    {
        CounterManager = counterManager;
    }

    public CounterItem CreateCounter(string operation)
    {
        var actionName = GetType().Name;
        if (actionName.EndsWith("Action"))
            actionName = actionName[..^"Action".Length];

        return CounterManager.Create($"Api.{actionName}.{operation}");
    }
}
