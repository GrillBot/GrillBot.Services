using ImageProcessingService.Caching.Models;
using ImageProcessingService.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ImageProcessingService.Caching;

public class WithoutAccidentCache : CacheBase
{
    public WithoutAccidentCache(IMemoryCache cache) : base(cache)
    {
    }

    public async Task<WithoutAccidentCacheData?> GetByRequestAsync(WithoutAccidentImageRequest request)
    {
        var cacheData = await ReadCacheAsync<WithoutAccidentCacheData>();

        // Remove all invalid items.
        cacheData.RemoveAll(o => o.DaysCount == request.DaysCount && o.UserId == request.UserId && o.AvatarId != request.AvatarInfo.AvatarId);
        return cacheData.Find(o => o.DaysCount == request.DaysCount && o.UserId == request.UserId && o.AvatarId == request.AvatarInfo.AvatarId);
    }

    public async Task WriteByRequestAsync(WithoutAccidentImageRequest request, byte[] image)
    {
        var cacheData = await ReadCacheAsync<WithoutAccidentCacheData>();
        var cacheItem = cacheData.Find(o => o.DaysCount == request.DaysCount && o.UserId == request.UserId && o.AvatarId == request.AvatarInfo.AvatarId);

        if (cacheItem is null)
        {
            cacheItem = new WithoutAccidentCacheData
            {
                AvatarId = request.AvatarInfo.AvatarId,
                DaysCount = request.DaysCount,
                UserId = request.UserId
            };

            cacheData.Add(cacheItem);
        }

        cacheItem.Image = image;
        cacheItem.ValidTo = DateTime.UtcNow.AddMonths(6);
        await UpdateCacheAsync(cacheData);
    }
}
