using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GrillBot.Services.Common.EntityFramework.Helpers;

public class ContextHelper<TDbContext>(
    ICounterManager _counterManager,
    TDbContext _dbContext,
    string _counterKey
) where TDbContext : DbContext
{
    public TDbContext DbContext => _dbContext;

    private CounterItem CreateCounter(string operation)
        => _counterManager.Create($"{_counterKey}.{operation}");

    public async Task<List<TEntity>> ReadEntitiesAsync<TEntity>(IQueryable<TEntity> query, CancellationToken cancellationToken = default)
    {
        using (CreateCounter("Database"))
            return await query.ToListAsync(cancellationToken);
    }

    public async Task<PaginatedResponse<TEntity>> ReadEntitiesWithPaginationAsync<TEntity>(IQueryable<TEntity> query, PaginatedParams parameters, CancellationToken cancellationToken = default) where TEntity : class
    {
        using (CreateCounter("Database"))
            return await PaginatedResponse<TEntity>.CreateWithEntityAsync(query, parameters, cancellationToken);
    }

    public async Task<TEntity> ReadFirstEntityAsync<TEntity>(IQueryable<TEntity> query, CancellationToken cancellationToken = default) where TEntity : class
    {
        using (CreateCounter("Database"))
            return await query.FirstAsync(cancellationToken);
    }

    public async Task<TEntity?> ReadFirstOrDefaultEntityAsync<TEntity>(IQueryable<TEntity> query, CancellationToken cancellationToken = default)
    {
        using (CreateCounter("Database"))
            return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<TEntity?> ReadFirstOrDefaultEntityAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default) where TEntity : class
        => ReadFirstOrDefaultEntityAsync(DbContext.Set<TEntity>().Where(whereExpression), cancellationToken);

    public async Task<int> ReadCountAsync<TEntity>(IQueryable<TEntity> query, CancellationToken cancellationToken = default) where TEntity : class
    {
        using (CreateCounter("Database"))
            return await query.CountAsync(cancellationToken);
    }

    public async Task<long> ReadSumAsync<TEntity>(IQueryable<TEntity> query, Expression<Func<TEntity, long>> selector, CancellationToken cancellationToken = default) where TEntity : class
    {
        using (CreateCounter("Database"))
            return await query.SumAsync(selector, cancellationToken);
    }

    public async Task<Dictionary<TKey, TValue>> ReadToDictionaryAsync<TEntity, TKey, TValue>(
        IQueryable<TEntity> query,
        Func<TEntity, TKey> keySelector,
        Func<TEntity, TValue> valueSelector,
        CancellationToken cancellationToken = default
    ) where TKey : notnull where TEntity : class
    {
        using (CreateCounter("Database"))
            return await query.ToDictionaryAsync(keySelector, valueSelector, cancellationToken);
    }

    public async Task<bool> IsAnyAsync<TEntity>(IQueryable<TEntity> query, CancellationToken cancellationToken = default) where TEntity : class
    {
        using (CreateCounter("Database"))
            return await query.AnyAsync(cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        using (CreateCounter("Database"))
            return await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> ExecuteBatchDeleteAsync<TEntity>(IQueryable<TEntity> query, CancellationToken cancellationToken = default) where TEntity : class
    {
        using (CreateCounter("Database"))
            return await query.ExecuteDeleteAsync(cancellationToken);
    }
}
