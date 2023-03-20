using PointsService.Core.Entity;

namespace PointsService.Core.Repository;

public class PointsServiceRepository
{
    private PointsServiceContext Context { get; }

    public ChannelRepository Channel { get; }
    public UserRepository User { get; }
    public TransactionRepository Transaction { get; }
    public StatisticsRepository Statistics { get; }

    public PointsServiceRepository(PointsServiceContext context)
    {
        Context = context;

        Channel = new ChannelRepository(context);
        User = new UserRepository(context);
        Transaction = new TransactionRepository(context);
        Statistics = new StatisticsRepository(context);
    }

    public Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        => Context.Set<TEntity>().AddAsync(entity).AsTask();

    public Task AddAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        => Context.Set<TEntity>().AddRangeAsync(entities);

    public void Remove<TEntity>(TEntity entity) where TEntity : class
        => Context.Set<TEntity>().Remove(entity);

    public void Remove<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        => Context.Set<TEntity>().RemoveRange(entities);

    public Task<int> CommitAsync()
        => Context.SaveChangesAsync();
}
