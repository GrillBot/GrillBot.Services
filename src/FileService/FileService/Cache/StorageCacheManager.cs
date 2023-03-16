using System.Diagnostics.CodeAnalysis;
using Azure.Storage.Sas;
using GrillBot.Core.Managers.Performance;
using Microsoft.Extensions.Caching.Memory;

namespace FileService.Cache;

public class StorageCacheManager
{
    private IMemoryCache Cache { get; }
    private ICounterManager CounterManager { get; }

    private static readonly object Locker = new();

    private static DateTimeOffset FileExpirationAt
        => DateTimeOffset.Now.AddDays(1);

    public StorageCacheManager(IMemoryCache memoryCache, ICounterManager counterManager)
    {
        Cache = memoryCache;
        CounterManager = counterManager;
    }

    public void Add(string filename, string contentType, byte[] content)
    {
        var cacheFile = new CacheFile
        {
            Content = content,
            ContentType = contentType
        };

        lock (Locker)
        {
            using (CounterManager.Create("Cache"))
            {
                Cache.Set(filename, cacheFile, FileExpirationAt);
            }
        }
    }

    public void AddSasLink(string filename, string link, BlobSasBuilder builder)
    {
        var cacheKey = $"[SAS]({filename})";

        lock (Locker)
        {
            using (CounterManager.Create("Cache"))
            {
                Cache.Set(cacheKey, link, builder.ExpiresOn);
            }
        }
    }

    public bool TryGet(string filename, out string contentType, out byte[] content)
    {
        content = Array.Empty<byte>();
        contentType = "";

        lock (Locker)
        {
            using (CounterManager.Create("Cache"))
            {
                if (!Cache.TryGetValue(filename, out CacheFile? cacheFile))
                    return false;

                content = cacheFile!.Content;
                contentType = cacheFile.ContentType;
                return true;
            }
        }
    }

    public bool TryGetSasLink(string filename, [MaybeNullWhen(false)] out string link)
    {
        link = null;
        var cacheKey = $"[SAS]({filename})";

        lock (Locker)
        {
            using (CounterManager.Create("Cache"))
            {
                return Cache.TryGetValue(cacheKey, out link);
            }
        }
    }

    public void Remove(string filename)
    {
        lock (Locker)
        {
            using (CounterManager.Create("Cache"))
            {
                Cache.Remove(filename);
            }
        }
    }

    public void RemoveSasLink(string filename)
    {
        lock (Locker)
        {
            using (CounterManager.Create("Cache"))
            {
                Cache.Remove($"[SAS]{filename}");
            }
        }
    }
}
