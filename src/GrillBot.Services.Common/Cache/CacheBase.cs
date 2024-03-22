using GrillBot.Core.Managers.Performance;

namespace GrillBot.Services.Common.Cache;

public abstract class CacheBase
{
    private readonly ICounterManager _counterManager;

    protected CacheBase(ICounterManager counterManager)
    {
        _counterManager = counterManager;
    }

    protected CounterItem CreateCounterItem(string operation)
        => _counterManager.Create($"Cache.{GetType().Name}.{operation}");
}
