using GrillBot.Core.Managers.Performance;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable S3604 // Member initializer values should not be redundant
namespace GrillBot.Services.Common.Cache.Abstraction;

internal interface IScopedCache;

public abstract class ScopedCache<TCacheKey, TCacheData>(
    ICounterManager counterManager
) : CacheBase(counterManager), IDisposable, IScopedCache where TCacheKey : notnull
{
    private readonly Dictionary<TCacheKey, TCacheData> _data = [];

    protected bool TryRead(TCacheKey key, [MaybeNullWhen(false)] out TCacheData data)
    {
        using (CreateCounterItem("TryRead"))
            return _data.TryGetValue(key, out data);
    }

    protected void Write(TCacheKey key, TCacheData data)
    {
        using (CreateCounterItem("Write"))
            _data[key] = data;
    }

    protected void Remove(TCacheKey key)
    {
        using (CreateCounterItem("Remove"))
            _data.Remove(key);
    }

    protected virtual void DisposeItem(TCacheData data) { }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
            return;

        foreach (var item in _data.Values)
            DisposeItem(item);
        _data.Clear();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
