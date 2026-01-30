using GrillBot.Core.Redis.Extensions;
using ImageProcessingService.Caching.Models;
using ImageProcessingService.Models;
using Microsoft.Extensions.Caching.Distributed;

namespace ImageProcessingService.Caching;

public class PointsCache(
    IDistributedCache _cache,
    ILogger<PointsCache> _logger
)
{
    private static readonly TimeSpan _cacheExpiration = TimeSpan.FromDays(1);

    public async Task<ImageCacheData?> GetByPointsRequestAsync(PointsRequest request)
    {
        var cacheKey = CreateCacheKey(request);

        try
        {
            return await _cache.GetAsync<ImageCacheData>(cacheKey);
        }
        catch (Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning(ex, "Cannot get points image from cache.");
            return null;
        }
    }

    public async Task WriteByPointsRequestAsync(PointsRequest request, byte[] image)
    {
        var cacheKey = CreateCacheKey(request);

        var cacheData = new ImageCacheData
        {
            ContentType = "image/png",
            Image = image
        };

        try
        {
            await _cache.SetAsync(cacheKey, cacheData, _cacheExpiration);
        }
        catch (Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
                _logger.LogWarning(ex, "Cannot write points image to cache.");
        }
    }

    private static string CreateCacheKey(PointsRequest request)
        => $"Points({request.UserId}; {request.Username}; {request.PointsValue}; {request.Position}; {request.AvatarInfo.AvatarId})";
}
