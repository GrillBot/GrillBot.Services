using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace GrillBot.Services.Common.EntityFramework.Helpers;

public class ContextHelper<TDbContext> where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;
    private readonly ICounterManager _counterManager;
    private readonly string _counterKey;

    public ContextHelper(ICounterManager counterManager, TDbContext dbContext, string counterKey)
    {
        _dbContext = dbContext;
        _counterManager = counterManager;
        _counterKey = counterKey;
    }

    private CounterItem CreateCounter(string operation)
        => _counterManager.Create($"{_counterKey}.{operation}");

    public async Task<List<TEntity>> ReadEntitiesAsync<TEntity>(IQueryable<TEntity> query)
    {
        using (CreateCounter("Database"))
            return await query.ToListAsync();
    }

    public async Task<PaginatedResponse<TEntity>> ReadEntitiesWithPaginationAsync<TEntity>(IQueryable<TEntity> query, PaginatedParams parameters) where TEntity : class
    {
        using (CreateCounter("Database"))
            return await PaginatedResponse<TEntity>.CreateWithEntityAsync(query, parameters);
    }

    public async Task<TEntity> ReadFirstEntityAsync<TEntity>(IQueryable<TEntity> query) where TEntity : class
    {
        using (CreateCounter("Database"))
            return await query.FirstAsync();
    }

    public async Task<TEntity?> ReadFirstOrDefaultEntityAsync<TEntity>(IQueryable<TEntity> query)
    {
        using (CreateCounter("Database"))
            return await query.FirstOrDefaultAsync();
    }

    public Task<TEntity?> ReadFirstOrDefaultEntityAsync<TEntity>(Expression<Func<TEntity, bool>> whereExpression) where TEntity : class
        => ReadFirstOrDefaultEntityAsync(_dbContext.Set<TEntity>().Where(whereExpression));

    public async Task<int> ReadCountAsync<TEntity>(IQueryable<TEntity> query) where TEntity : class
    {
        using (CreateCounter("Database"))
            return await query.CountAsync();
    }

    public async Task<Dictionary<TKey, TValue>> ReadToDictionaryAsync<TEntity, TKey, TValue>(IQueryable<TEntity> query, Func<TEntity, TKey> keySelector, Func<TEntity, TValue> valueSelector) where TKey : notnull where TEntity : class
    {
        using (CreateCounter("Database"))
            return await query.ToDictionaryAsync(keySelector, valueSelector);
    }

    public async Task<bool> IsAnyAsync<TEntity>(IQueryable<TEntity> query) where TEntity : class
    {
        using (CreateCounter("Database"))
            return await query.AnyAsync();
    }

    public async Task<int> SaveChagesAsync()
    {
        using (CreateCounter("Database"))
            return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> ExecuteBatchDeleteAsync<TEntity>(IQueryable<TEntity> query) where TEntity : class
    {
        using (CreateCounter("Database"))
            return await query.ExecuteDeleteAsync();
    }
}
