using GrillBot.Core.Managers.Performance;

namespace AuditLogService.Core.Discord.Cache;

public abstract class GuildCacheBase<TData> : IDisposable
{
    protected static readonly object Locker = new();
    protected readonly Dictionary<ulong, TData> CachedData = new();

    protected ICounterManager CounterManager { get; }

    protected GuildCacheBase(ICounterManager counterManager)
    {
        CounterManager = counterManager;
    }

    protected abstract void DisposeItem(TData data);

    private void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        lock (Locker)
        {
            foreach (var item in CachedData.Values)
                DisposeItem(item);
            CachedData.Clear();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
