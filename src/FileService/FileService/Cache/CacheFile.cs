namespace FileService.Cache;

public class CacheFile
{
    public string ContentType { get; set; } = null!;
    public byte[] Content { get; set; } = null!;
}
