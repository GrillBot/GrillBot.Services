using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using FileService.Cache;
using GrillBot.Core.Infrastructure.Actions;

namespace FileService.Actions;

public class GenerateLinkAction : ApiActionBase
{
    private StorageCacheManager Cache { get; }
    private BlobContainerClient Client { get; }

    public GenerateLinkAction(StorageCacheManager cache, BlobContainerClient client)
    {
        Cache = cache;
        Client = client;
    }

    public override Task<ApiResult> ProcessAsync()
    {
        var filename = (string)Parameters[0]!;

        if (Cache.TryGetSasLink(filename, out var link))
            return Task.FromResult(new ApiResult(StatusCodes.Status200OK, link));

        var blobClient = Client.GetBlobClient(filename);
        if (!blobClient.CanGenerateSasUri)
            return Task.FromResult(new ApiResult(StatusCodes.Status404NotFound));

        var builder = new BlobSasBuilder
        {
            BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
            BlobName = blobClient.Name,
            Resource = "b",
            ExpiresOn = DateTimeOffset.Now.AddHours(1)
        };
        builder.SetPermissions(BlobSasPermissions.Read);

        link = blobClient.GenerateSasUri(builder).ToString();
        Cache.AddSasLink(filename, link, builder);

        return Task.FromResult(new ApiResult(StatusCodes.Status200OK, link));
    }
}
