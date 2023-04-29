namespace ImageProcessingService.Caching.Models;

public class PointsCacheData : CacheItemBase
{
    public string UserId { get; set; } = null!;
    public string Username { get; set; } = null!;
    public int Points { get; set; }
    public int Position { get; set; }
    public string AvatarId { get; set; } = null!;

    public byte[] Image { get; set; } = null!;
}
