using GrillBot.Core.Infrastructure.Actions;
using ImageMagick;
using ImageProcessingService.Caching;
using ImageProcessingService.Models;
using ImageProcessingService.Renderers;
using Microsoft.AspNetCore.Mvc;

namespace ImageProcessingService.Actions;

public class PeepoLoveAction : ApiActionBase
{
    private readonly PeepoCache _cache;

    public PeepoLoveAction(PeepoCache cache)
    {
        _cache = cache;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<PeepoRequest>(0);

        var cachedImage = await _cache.GetByPeepoRequestAsync(request, "PeepoLove");
        if (cachedImage is not null)
            return CreateResult(cachedImage.Image, cachedImage.ContentType);

        var profilePictureFrames = request.GetProfilePictureFrames();

        byte[] resultImage;
        string resultContentType;

        var createdFrames = PeepoloveRenderer.Render(profilePictureFrames);
        try
        {
            if (createdFrames.Count == 1)
            {
                resultImage = createdFrames[0].ToByteArray(MagickFormat.Png);
                resultContentType = "image/png";
            }
            else
            {
                var framesQuery = createdFrames.Select(rawFrame =>
                {
                    var frameData = rawFrame.ToByteArray(MagickFormat.Png);
                    return new MagickImage(frameData, MagickFormat.Png)
                    {
                        GifDisposeMethod = GifDisposeMethod.Background
                    };
                });

                using var collection = new MagickImageCollection(framesQuery);

                resultImage = collection.ToByteArray(MagickFormat.Gif);
                resultContentType = "image/gif";
            }
        }
        finally
        {
            foreach (var frame in createdFrames)
                frame.Dispose();

            foreach (var profilePicture in profilePictureFrames)
                profilePicture.Dispose();
        }

        await _cache.WriteByPeepoRequest(request, "PeepoLove", resultImage, resultContentType);
        return CreateResult(resultImage, resultContentType);
    }

    private static ApiResult CreateResult(byte[] image, string contentType)
        => ApiResult.Ok(new FileContentResult(image, contentType));
}
