using GrillBot.Core.Managers.Performance;

namespace AuditLogService.Cache.Abstraction;

public abstract class CacheBase
{
    protected ICounterManager CounterManager { get; }

    private string CacheName => GetType().Name;

    protected CacheBase(ICounterManager counterManager)
    {
        CounterManager = counterManager;
    }

    protected CounterItem CreateCounterItem(string action)
        => CounterManager.Create($"Cache.{CacheName}.{action}");
}
