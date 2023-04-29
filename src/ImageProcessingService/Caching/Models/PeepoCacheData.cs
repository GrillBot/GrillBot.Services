namespace ImageProcessingService.Caching.Models;

public class PeepoCacheData : CacheItemBase
{
    public string UserId { get; set; } = null!;
    public string AvatarId { get; set; } = null!;
    public string PeepoImageType { get; set; } = null!;

    public byte[] Image { get; set; } = null!;
    public string ContentType { get; set; } = null!;
}
