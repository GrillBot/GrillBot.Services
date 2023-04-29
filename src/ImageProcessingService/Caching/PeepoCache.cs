using ImageProcessingService.Caching.Models;
using ImageProcessingService.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ImageProcessingService.Caching;

public class PeepoCache : CacheBase
{
    public PeepoCache(IMemoryCache cache) : base(cache)
    {
    }

    public async Task<PeepoCacheData?> GetByPeepoRequestAsync(PeepoRequest request, string peepoImageType)
    {
        var cacheData = await ReadCacheAsync<PeepoCacheData>();

        // Remove all invalid records.
        cacheData.RemoveAll(o => o.PeepoImageType == peepoImageType && o.UserId == request.UserId && o.AvatarId != request.AvatarInfo.AvatarId);
        return cacheData.Find(o => o.PeepoImageType == peepoImageType && o.UserId == request.UserId && o.AvatarId == request.AvatarInfo.AvatarId);
    }

    public async Task WriteByPeepoRequest(PeepoRequest request, string peepoImageType, byte[] image, string contentType)
    {
        var cacheData = await ReadCacheAsync<PeepoCacheData>();
        var cacheItem = cacheData.Find(o => o.PeepoImageType == peepoImageType && o.UserId == request.UserId && o.AvatarId == request.AvatarInfo.AvatarId);

        if (cacheItem is null)
        {
            cacheItem = new PeepoCacheData
            {
                AvatarId = request.AvatarInfo.AvatarId,
                UserId = request.UserId,
                PeepoImageType = peepoImageType
            };

            cacheData.Add(cacheItem);
        }

        cacheItem.ValidTo = DateTime.UtcNow.AddMonths(1);
        cacheItem.Image = image;
        cacheItem.ContentType = contentType;
        await UpdateCacheAsync(cacheData);
    }
}
