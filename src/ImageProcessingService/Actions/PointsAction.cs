using System.Drawing;
using GrillBot.Core.Infrastructure.Actions;
using ImageMagick;
using ImageProcessingService.Caching;
using ImageProcessingService.Core.GraphicsService;
using ImageProcessingService.Core.GraphicsService.Models.Images;
using ImageProcessingService.Models;
using Microsoft.AspNetCore.Mvc;

namespace ImageProcessingService.Actions;

public class PointsAction : ApiActionBase
{
    private PointsCache Cache { get; }
    private IGraphicsClient GraphicsClient { get; }

    public PointsAction(PointsCache cache, IGraphicsClient graphicsClient)
    {
        Cache = cache;
        GraphicsClient = graphicsClient;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (PointsRequest)Parameters[0]!;

        var cacheData = await Cache.GetByPointsRequestAsync(request);
        if (cacheData is not null)
            return CreateResult(cacheData.Image);

        using var profilePicture = new MagickImage(request.AvatarInfo.AvatarContent);

        var dominantColor = GetDominantColor(profilePicture);
        var textBackground = CreateDarkerBackgroundColor(dominantColor);

        var imageRequest = new PointsImageRequest
        {
            Nickname = request.Username,
            Points = request.PointsValue,
            Position = request.Position,
            BackgroundColor = dominantColor.ToHexString(),
            ProfilePicture = profilePicture.ToBase64(),
            TextBackground = textBackground.ToHexString()
        };

        var image = await GraphicsClient.CreatePointsImageAsync(imageRequest);

        await Cache.WriteByPointsRequestAsync(request, image);
        return CreateResult(image);
    }

    private static MagickColor GetDominantColor(MagickImage image)
    {
        using var clone = image.Clone();
        clone.HasAlpha = false;

        var histogram = clone.Histogram();
        return new MagickColor(histogram.Aggregate((x, y) => x.Value > y.Value ? x : y).Key);
    }

    private static MagickColor CreateDarkerBackgroundColor(MagickColor color)
    {
        var tmpColor = Color.FromArgb(color.A, color.R, color.G, color.B);
        tmpColor = tmpColor.GetBrightness() <= 0.2 ? Color.FromArgb(25, Color.White) : Color.FromArgb(100, Color.Black);

        return MagickColor.FromRgba(tmpColor.R, tmpColor.G, tmpColor.B, tmpColor.A);
    }

    private static ApiResult CreateResult(byte[] image)
        => ApiResult.FromSuccess(new FileContentResult(image, "image/png"));
}
