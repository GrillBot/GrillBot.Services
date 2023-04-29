namespace ImageProcessingService.Caching.Models;

public class ChartCacheData : CacheItemBase
{
    public string Hash { get; set; } = null!;
    public byte[] Image { get; set; } = null!;
}
