namespace ImageProcessingService.Caching.Models;

public class ImageCacheData
{
    public required byte[] Image { get; set; }
    public required string ContentType { get; set; }
}
