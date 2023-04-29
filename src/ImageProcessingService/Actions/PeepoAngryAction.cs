using GrillBot.Core.Infrastructure.Actions;
using ImageMagick;
using ImageProcessingService.Caching;
using ImageProcessingService.Core.GraphicsService;
using ImageProcessingService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ImageProcessingService.Actions;

public class PeepoAngryAction : ApiActionBase
{
    private PeepoCache Cache { get; }
    private IGraphicsClient GraphicsClient { get; }

    public PeepoAngryAction(PeepoCache cache, IGraphicsClient graphicsClient)
    {
        Cache = cache;
        GraphicsClient = graphicsClient;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (PeepoRequest)Parameters[0]!;

        var cachedImage = await Cache.GetByPeepoRequestAsync(request, "PeepoAngry");
        if (cachedImage is not null)
            return new ApiResult(StatusCodes.Status200OK, new FileContentResult(cachedImage.Image, cachedImage.ContentType));

        var isAnimated = request.IsAnimated();
        var profilePictureFrames = new List<byte[]>();

        using var profilePicture = new MagickImageCollection(request.AvatarInfo.AvatarContent);

        if (isAnimated)
        {
            profilePicture.Coalesce();
            profilePictureFrames.AddRange(profilePicture.Select(frame => frame.ToByteArray()));
        }
        else
        {
            profilePictureFrames.Add(profilePicture[0].ToByteArray());
        }

        byte[] resultImage;
        string resultContentType;

        var createdFrames = await GraphicsClient.CreatePeepoAngryAsync(profilePictureFrames);
        if (createdFrames.Count == 1)
        {
            using var img = new MagickImage(createdFrames[0]);

            resultImage = img.ToByteArray(MagickFormat.Png);
            resultContentType = "image/png";
        }
        else
        {
            var framesQuery = createdFrames.Select(frameData =>
            {
                var frame = new MagickImage(frameData, MagickFormat.Png);
                frame.GifDisposeMethod = GifDisposeMethod.Background;

                return frame;
            });

            using var collection = new MagickImageCollection(framesQuery);

            resultImage = collection.ToByteArray(MagickFormat.Gif);
            resultContentType = "image/gif";
        }

        await Cache.WriteByPeepoRequest(request, "PeepoAngry", resultImage, resultContentType);
        return new ApiResult(StatusCodes.Status200OK, new FileContentResult(resultImage, resultContentType));
    }
}
