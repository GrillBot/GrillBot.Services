namespace ImageProcessingService.Caching.Models;

public class WithoutAccidentCacheData : CacheItemBase
{
    public string UserId { get; set; } = null!;
    public string AvatarId { get; set; } = null!;
    public int DaysCount { get; set; }
    public byte[] Image { get; set; } = null!;
}
