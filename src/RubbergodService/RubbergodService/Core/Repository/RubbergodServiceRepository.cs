using GrillBot.Core.Managers.Performance;
using RubbergodService.Core.Entity;

namespace RubbergodService.Core.Repository;

public sealed class RubbergodServiceRepository : IDisposable, IAsyncDisposable
{
    private RubbergodServiceContext Context { get; set; }
    private ICounterManager CounterManager { get; }

    public RubbergodServiceRepository(RubbergodServiceContext context, ICounterManager counterManager)
    {
        Context = context;
        CounterManager = counterManager;

        Karma = new KarmaRepository(Context, counterManager);
        MemberCache = new MemberCacheRepository(Context, counterManager);
        Statistics = new StatisticsRepository(Context, counterManager);
    }

    public KarmaRepository Karma { get; }
    public MemberCacheRepository MemberCache { get; }
    public StatisticsRepository Statistics { get; }

    public Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        => Context.Set<TEntity>().AddAsync(entity).AsTask();

    public Task<int> CommitAsync()
    {
        using (CounterManager.Create("Repository.Commit"))
        {
            return Context.SaveChangesAsync();
        }
    }

    public void Dispose()
    {
        Context.ChangeTracker.Clear();
        Context.Dispose();
        Context = null!;
    }

    public async ValueTask DisposeAsync()
    {
        Context.ChangeTracker.Clear();
        await Context.DisposeAsync();
        Context = null!;
    }
}
