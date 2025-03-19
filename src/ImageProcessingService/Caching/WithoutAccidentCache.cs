using GrillBot.Core.Redis.Extensions;
using ImageProcessingService.Caching.Models;
using ImageProcessingService.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace ImageProcessingService.Caching;

public class WithoutAccidentCache(IDistributedCache _cache)
{
    private static readonly TimeSpan _cacheExpiration = TimeSpan.FromDays(6 * 31);

    public async Task<ImageCacheData?> GetByRequestAsync(WithoutAccidentImageRequest request)
    {
        var cacheKey = CreateCacheKey(request);
        return await _cache.GetAsync<ImageCacheData>(cacheKey);
    }

    public async Task WriteByRequestAsync(WithoutAccidentImageRequest request, byte[] image)
    {
        var cacheKey = CreateCacheKey(request);

        var cacheData = new ImageCacheData
        {
            ContentType = "image/png",
            Image = image
        };

        await _cache.SetAsync(cacheKey, cacheData, _cacheExpiration);
    }

    private static string CreateCacheKey(WithoutAccidentImageRequest request)
        => $"WithoutAccident({request.DaysCount}; {request.UserId}; {request.AvatarInfo.AvatarId})";
}
