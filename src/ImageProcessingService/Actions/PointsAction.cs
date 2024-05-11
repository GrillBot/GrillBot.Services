using GrillBot.Core.Infrastructure.Actions;
using ImageMagick;
using ImageProcessingService.Caching;
using ImageProcessingService.Models;
using ImageProcessingService.Renderers;
using Microsoft.AspNetCore.Mvc;

namespace ImageProcessingService.Actions;

public class PointsAction : ApiActionBase
{
    private readonly PointsCache _cache;

    public PointsAction(PointsCache cache)
    {
        _cache = cache;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<PointsRequest>(0);

        var cacheData = await _cache.GetByPointsRequestAsync(request);
        if (cacheData is not null)
            return CreateResult(cacheData.Image);

        using var profilePicture = new MagickImage(request.AvatarInfo.AvatarContent);
        using var image = PointsImageRenderer.Render(profilePicture, request.Username, request.PointsValue, request.Position);

        var imageData = image.ToByteArray(MagickFormat.Png);

        await _cache.WriteByPointsRequestAsync(request, imageData);
        return CreateResult(imageData);
    }



    private static ApiResult CreateResult(byte[] image)
        => ApiResult.Ok(new FileContentResult(image, "image/png"));
}
