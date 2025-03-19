using GrillBot.Core.Redis.Extensions;
using ImageProcessingService.Caching.Models;
using ImageProcessingService.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace ImageProcessingService.Caching;

public class PointsCache(IDistributedCache _cache)
{
    private static readonly TimeSpan _cacheExpiration = TimeSpan.FromDays(1);

    public async Task<ImageCacheData?> GetByPointsRequestAsync(PointsRequest request)
    {
        var cacheKey = CreateCacheKey(request);
        return await _cache.GetAsync<ImageCacheData>(cacheKey);
    }

    public async Task WriteByPointsRequestAsync(PointsRequest request, byte[] image)
    {
        var cacheKey = CreateCacheKey(request);

        var cacheData = new ImageCacheData
        {
            ContentType = "image/png",
            Image = image
        };

        await _cache.SetAsync(cacheKey, cacheData, _cacheExpiration);
    }

    private static string CreateCacheKey(PointsRequest request)
        => $"Points({request.UserId}; {request.Username}; {request.PointsValue}; {request.Position}; {request.AvatarInfo.AvatarId})";
}
