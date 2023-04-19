using GrillBot.Core.Managers.Performance;
using Microsoft.EntityFrameworkCore;
using PointsService.Core.Entity;

namespace PointsService.Core.Repository;

public class PointsServiceRepository
{
    private PointsServiceContext Context { get; }
    private ICounterManager CounterManager { get; }

    public ChannelRepository Channel { get; }
    public UserRepository User { get; }
    public TransactionRepository Transaction { get; }
    public StatisticsRepository Statistics { get; }
    public LeaderboardRepository Leaderboard { get; }

    public PointsServiceRepository(PointsServiceContext context, ICounterManager counterManager)
    {
        Context = context;
        CounterManager = counterManager;

        Channel = new ChannelRepository(context, counterManager);
        User = new UserRepository(context, counterManager);
        Transaction = new TransactionRepository(context, counterManager);
        Statistics = new StatisticsRepository(context, counterManager);
        Leaderboard = new LeaderboardRepository(context, counterManager);
    }

    public Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        => Context.Set<TEntity>().AddAsync(entity).AsTask();

    public Task AddCollectionAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        => Context.Set<TEntity>().AddRangeAsync(entities);

    public void Remove<TEntity>(TEntity entity) where TEntity : class
        => Context.Set<TEntity>().Remove(entity);

    public void RemoveCollection<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        => Context.Set<TEntity>().RemoveRange(entities);

    public async Task<int> CommitAsync()
    {
        using (CounterManager.Create("Repository.Commit"))
        {
            return await Context.SaveChangesAsync();
        }
    }

    public async Task<bool> IsPendingMigrationsAsync()
    {
        using (CounterManager.Create("Repository.Migrations"))
        {
            return (await Context.Database.GetPendingMigrationsAsync()).Any();
        }
    }
}
