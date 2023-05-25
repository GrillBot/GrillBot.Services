using GrillBot.Core.Infrastructure.Actions;
using ImageMagick;
using ImageProcessingService.Caching;
using ImageProcessingService.Core.GraphicsService;
using ImageProcessingService.Core.GraphicsService.Models.Images;
using ImageProcessingService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ImageProcessingService.Actions;

public class WithoutAccidentAction : ApiActionBase
{
    private WithoutAccidentCache Cache { get; }
    private IGraphicsClient GraphicsClient { get; }

    public WithoutAccidentAction(WithoutAccidentCache cache, IGraphicsClient graphicsClient)
    {
        Cache = cache;
        GraphicsClient = graphicsClient;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (WithoutAccidentImageRequest)Parameters[0]!;

        var cacheItem = await Cache.GetByRequestAsync(request);
        if (cacheItem is not null)
            return new ApiResult(StatusCodes.Status200OK, new FileContentResult(cacheItem.Image, "image/png"));

        using var profilePicture = new MagickImage(request.AvatarInfo.AvatarContent);
        profilePicture.Resize(512, 512);

        var imageRequest = new WithoutAccidentRequestData
        {
            Days = request.DaysCount,
            ProfilePicture = profilePicture.ToBase64()
        };

        var image = await GraphicsClient.CreateWithoutAccidentImageAsync(imageRequest);

        await Cache.WriteByRequestAsync(request, image);
        return new ApiResult(StatusCodes.Status200OK, new FileContentResult(image, "image/png"));
    }
}
