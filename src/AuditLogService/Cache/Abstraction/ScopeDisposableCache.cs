using GrillBot.Core.Managers.Performance;

namespace AuditLogService.Cache.Abstraction;

/// <summary>
/// A cache that exists only within the scope of the request.
/// </summary>
public abstract class ScopeDisposableCache<TCacheKey, TCacheData> : CacheBase, IDisposable where TCacheKey : notnull
{
    protected Dictionary<TCacheKey, TCacheData> CacheData { get; }

    protected ScopeDisposableCache(ICounterManager counterManager) : base(counterManager)
    {
        CacheData = new Dictionary<TCacheKey, TCacheData>();
    }

    protected TCacheData? Read(TCacheKey key)
    {
        using (CreateCounterItem("Read"))
        {
            return CacheData.TryGetValue(key, out var value) ? value : default;
        }
    }

    protected void Write(TCacheKey key, TCacheData data)
    {
        using (CreateCounterItem("Write"))
        {
            CacheData[key] = data;
        }
    }

    protected abstract void DisposeItem(TCacheData data);

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        foreach (var item in CacheData.Values)
            DisposeItem(item);
        CacheData.Clear();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
