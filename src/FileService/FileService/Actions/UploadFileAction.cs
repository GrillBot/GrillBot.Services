using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;

namespace FileService.Actions;

public class UploadFileAction : ApiActionBase
{
    private ICounterManager CounterManager { get; }
    private BlobContainerClient Client { get; }

    public UploadFileAction(ICounterManager counterManager, BlobContainerClient client)
    {
        CounterManager = counterManager;
        Client = client;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var file = (IFormFile)Parameters[0]!;
        await using var stream = file.OpenReadStream();

        try
        {
            using (CounterManager.Create("Azure.Storage.Upload"))
            {
                await Client.UploadBlobAsync(file.FileName, stream);
            }

            return new ApiResult(StatusCodes.Status200OK);
        }
        catch (RequestFailedException ex) when (ex.ErrorCode == BlobErrorCode.BlobAlreadyExists)
        {
            return new ApiResult(StatusCodes.Status409Conflict);
        }
    }
}
