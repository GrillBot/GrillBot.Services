using ImageProcessingService.Caching.Models;
using ImageProcessingService.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ImageProcessingService.Caching;

public class ChartCache : CacheBase
{
    public ChartCache(IMemoryCache cache) : base(cache)
    {
    }

    public async Task<ChartCacheData?> GetByChartRequestAsync(ChartRequest request)
    {
        var cacheData = await ReadCacheAsync<ChartCacheData>();

        var hash = request.GetHash();
        return cacheData.Find(o => o.Hash == hash);
    }

    public async Task WriteByChartRequestAsync(ChartRequest request, byte[] image)
    {
        var hash = request.GetHash();
        var cacheData = await ReadCacheAsync<ChartCacheData>();
        var cacheItem = cacheData.Find(o => o.Hash == hash);

        if (cacheItem is null)
        {
            cacheItem = new ChartCacheData { Hash = hash };
            cacheData.Add(cacheItem);
        }

        cacheItem.Image = image;
        cacheItem.ValidTo = DateTime.UtcNow.AddDays(1);
        await UpdateCacheAsync(cacheData);
    }
}
