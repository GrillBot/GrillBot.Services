using GrillBot.Core.Managers.Performance;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics.CodeAnalysis;

namespace GrillBot.Services.Common.Cache.Abstraction;

internal interface IInMemoryCache;

public abstract class InMemoryCache<TCacheData>(
    ICounterManager counterManager,
    IMemoryCache _memoryCache
) : CacheBase(counterManager), IInMemoryCache
{
    protected bool TryReadData(string key, [MaybeNullWhen(false)] out TCacheData data)
    {
        using (CreateCounterItem("TryRead"))
            return _memoryCache.TryGetValue(key, out data);
    }

    protected void Write(string key, TCacheData data, DateTimeOffset? expireAt = null)
    {
        expireAt ??= DateTimeOffset.MaxValue;

        using (CreateCounterItem("Write"))
            _memoryCache.Set(key, data, expireAt.Value);
    }

    protected void Remove(string key)
    {
        using (CreateCounterItem("Remove"))
            _memoryCache.Remove(key);
    }
}
