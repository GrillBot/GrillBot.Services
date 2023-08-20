using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Services.Graphics;
using GrillBot.Core.Services.Graphics.Models.Images;
using ImageMagick;
using ImageProcessingService.Caching;
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
            return CreateResult(cacheItem.Image);

        using var profilePicture = new MagickImage(request.AvatarInfo.AvatarContent);
        profilePicture.Resize(512, 512);

        var imageRequest = new WithoutAccidentRequestData
        {
            Days = request.DaysCount,
            ProfilePicture = profilePicture.ToBase64()
        };

        var image = await GraphicsClient.CreateWithoutAccidentImage(imageRequest);

        await Cache.WriteByRequestAsync(request, image);
        return CreateResult(image);
    }

    private static ApiResult CreateResult(byte[] image)
        => ApiResult.FromSuccess(new FileContentResult(image, "image/png"));
}
