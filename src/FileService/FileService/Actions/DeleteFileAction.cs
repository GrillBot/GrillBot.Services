using Azure.Storage.Blobs;
using FileService.Cache;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;

namespace FileService.Actions;

public class DeleteFileAction : ApiActionBase
{
    private ICounterManager CounterManager { get; }
    private BlobContainerClient Client { get; }
    private StorageCacheManager Cache { get; }

    public DeleteFileAction(ICounterManager counterManager, BlobContainerClient client, StorageCacheManager cache)
    {
        CounterManager = counterManager;
        Client = client;
        Cache = cache;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var filename = (string)Parameters[0]!;

        using (CounterManager.Create("Azure.Storage.Delete"))
        {
            var blobClient = Client.GetBlobClient(filename);
            await blobClient.DeleteIfExistsAsync();
        }

        Cache.Remove(filename);
        Cache.RemoveSasLink(filename);
        return ApiResult.FromSuccess();
    }
}
