namespace ImageProcessingService.Caching.Models;

public abstract class CacheItemBase
{
    public DateTime ValidTo { get; set; }

    public bool IsValid()
        => ValidTo >= DateTime.UtcNow;
}
