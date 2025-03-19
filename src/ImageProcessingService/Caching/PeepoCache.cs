using GrillBot.Core.Redis.Extensions;
using ImageProcessingService.Caching.Models;
using ImageProcessingService.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace ImageProcessingService.Caching;

public class PeepoCache(IDistributedCache _cache)
{
    private static readonly TimeSpan _cacheExpiration = TimeSpan.FromDays(31);

    public async Task<ImageCacheData?> GetByPeepoRequestAsync(PeepoRequest request, string peepoImageType)
    {
        var cacheKey = CreateCacheKey(request, peepoImageType);
        return await _cache.GetAsync<ImageCacheData>(cacheKey);
    }

    public async Task WriteByPeepoRequest(PeepoRequest request, string peepoImageType, byte[] image, string contentType)
    {
        var cacheKey = CreateCacheKey(request, peepoImageType);

        var cacheData = new ImageCacheData
        {
            ContentType = contentType,
            Image = image
        };

        await _cache.SetAsync(cacheKey, cacheData, _cacheExpiration);
    }

    private static string CreateCacheKey(PeepoRequest request, string peepoImageType)
        => $"Peepo({peepoImageType}; {request.UserId}; {request.AvatarInfo.AvatarId})";
}
