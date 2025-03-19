using GrillBot.Core.Managers.Performance;

namespace GrillBot.Services.Common.Cache.Abstraction;

public abstract class CacheBase(ICounterManager _counterManager)
{
    protected CounterItem CreateCounterItem(string operation)
        => _counterManager.Create($"Cache.{GetType().Name}.{operation}");
}
