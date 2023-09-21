using GrillBot.Core.Managers.Performance;
using Microsoft.Extensions.Caching.Memory;

namespace AuditLogService.Cache.Abstraction;

/// <summary>
/// A cache that is not freed when a request completes.
/// </summary>
public abstract class InMemoryPersistentCache<TCacheData> : CacheBase
{
    private IMemoryCache Cache { get; }

    protected InMemoryPersistentCache(IMemoryCache cache, ICounterManager counterManager) : base(counterManager)
    {
        Cache = cache;
    }

    protected TCacheData? Read(string key)
    {
        using (CreateCounterItem("Read"))
        {
            return Cache.TryGetValue(key, out TCacheData? data) ? data : default;
        }
    }

    protected void Write(string key, TCacheData data, DateTimeOffset? expireAt)
    {
        expireAt ??= DateTimeOffset.MaxValue;

        using (CreateCounterItem("Write"))
        {
            Cache.Set(key, data, expireAt.Value);
        }
    }
}
