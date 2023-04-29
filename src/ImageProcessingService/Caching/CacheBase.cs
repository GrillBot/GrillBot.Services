using ImageProcessingService.Caching.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ImageProcessingService.Caching;

public abstract class CacheBase
{
    private IMemoryCache Cache { get; }
    private SemaphoreSlim SemaphoreSlim { get; }
    private string CacheName => GetType().Name;

    protected CacheBase(IMemoryCache cache)
    {
        Cache = cache;
        SemaphoreSlim = new SemaphoreSlim(1);
    }

    protected async Task<List<TCacheRecord>> ReadCacheAsync<TCacheRecord>() where TCacheRecord : CacheItemBase
    {
        await SemaphoreSlim.WaitAsync();
        try
        {
            if (!Cache.TryGetValue(CacheName, out List<TCacheRecord>? records))
                return new List<TCacheRecord>();

            records ??= new List<TCacheRecord>();
            records = records.FindAll(o => o.IsValid());
            Cache.Set(CacheName, records, DateTimeOffset.MaxValue);

            return records;
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }

    protected async Task UpdateCacheAsync<TCacheRecord>(List<TCacheRecord> records) where TCacheRecord : CacheItemBase
    {
        await SemaphoreSlim.WaitAsync();
        try
        {
            Cache.Set(CacheName, records, DateTimeOffset.MaxValue);
        }
        finally
        {
            SemaphoreSlim.Release();
        }
    }
}
