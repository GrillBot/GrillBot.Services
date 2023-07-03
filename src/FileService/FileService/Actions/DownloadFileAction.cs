using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FileService.Cache;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Actions;

public class DownloadFileAction : ApiActionBase
{
    private BlobContainerClient Client { get; }
    private StorageCacheManager Cache { get; }
    private ICounterManager CounterManager { get; }

    public DownloadFileAction(BlobContainerClient client, StorageCacheManager cache, ICounterManager counterManager)
    {
        Client = client;
        Cache = cache;
        CounterManager = counterManager;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var filename = (string)Parameters[0]!;

        if (Cache.TryGet(filename, out var contentType, out var content))
            return CreateResult(content, contentType, filename);

        var blobClient = Client.GetBlobClient(filename);

        try
        {
            using (CounterManager.Create("Azure.Storage.Download"))
            {
                var response = await blobClient.DownloadContentAsync();
                if (!response.HasValue)
                    return new ApiResult(StatusCodes.Status404NotFound);

                content = response.Value.Content.ToArray();
                contentType = response.Value.Details.ContentType;

                Cache.Add(filename, contentType, content);
                return CreateResult(content, contentType, filename);
            }
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobNotFound)
        {
            return new ApiResult(StatusCodes.Status404NotFound);
        }
    }

    private static ApiResult CreateResult(byte[] content, string contentType, string filename)
        => ApiResult.FromSuccess(new FileContentResult(content, contentType) { FileDownloadName = filename });
}
