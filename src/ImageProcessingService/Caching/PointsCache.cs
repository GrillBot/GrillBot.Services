using ImageProcessingService.Caching.Models;
using ImageProcessingService.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ImageProcessingService.Caching;

public class PointsCache : CacheBase
{
    public PointsCache(IMemoryCache cache) : base(cache)
    {
    }

    public async Task<PointsCacheData?> GetByPointsRequestAsync(PointsRequest request)
    {
        var cacheData = await ReadCacheAsync<PointsCacheData>();

        // Remove all invalid records.
        cacheData.RemoveAll(o => o.UserId == request.UserId && CanDelete(o, request));
        return cacheData.Find(o => o.UserId == request.UserId && !CanDelete(o, request));
    }

    private static bool CanDelete(PointsCacheData cache, PointsRequest request)
        => cache.Username != request.Username || cache.Points != request.PointsValue || cache.Position != request.Position || cache.AvatarId != request.AvatarInfo.AvatarId;

    public async Task WriteByPointsRequestAsync(PointsRequest request, byte[] image)
    {
        var cacheData = await ReadCacheAsync<PointsCacheData>();
        var cacheItem = cacheData.Find(o => o.UserId == request.UserId && !CanDelete(o, request));

        if (cacheItem is null)
        {
            cacheItem = new PointsCacheData
            {
                UserId = request.UserId,
                Position = request.Position,
                Username = request.Username,
                AvatarId = request.AvatarInfo.AvatarId,
                Points = request.PointsValue
            };

            cacheData.Add(cacheItem);
        }

        cacheItem.ValidTo = DateTime.UtcNow.AddDays(1);
        cacheItem.Image = image;
        await UpdateCacheAsync(cacheData);
    }
}
