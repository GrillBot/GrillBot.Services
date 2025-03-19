using GrillBot.Core.Infrastructure.Actions;
using ImageMagick;
using ImageProcessingService.Caching;
using ImageProcessingService.Models;
using ImageProcessingService.Renderers;
using Microsoft.AspNetCore.Mvc;

namespace ImageProcessingService.Actions;

public class WithoutAccidentAction(WithoutAccidentCache _cache) : ApiActionBase
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<WithoutAccidentImageRequest>(0);

        var cacheItem = await _cache.GetByRequestAsync(request);
        if (cacheItem is not null)
            return CreateResult(cacheItem.Image);

        using var profilePicture = new MagickImage(request.AvatarInfo.AvatarContent);
        using var result = WithoutAccidentRenderer.Render(profilePicture, request.DaysCount);

        var image = result.ToByteArray(MagickFormat.Png);

        await _cache.WriteByRequestAsync(request, image);
        return CreateResult(image);
    }

    private static ApiResult CreateResult(byte[] image)
        => ApiResult.Ok(new FileContentResult(image, "image/png"));
}
