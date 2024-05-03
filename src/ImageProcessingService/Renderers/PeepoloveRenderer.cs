using ImageMagick;
using ImageProcessingService.Extensions;
using ImageProcessingService.Resources;

namespace ImageProcessingService.Renderers;

public static class PeepoloveRenderer
{
    public static List<MagickImage> Render(List<IMagickImage<byte>> profilePictureFrames)
    {
        if (profilePictureFrames.Count == 0)
            throw new ArgumentException("Missing profile pictures.", nameof(profilePictureFrames));

        if (profilePictureFrames.Count > 1)
            return RenderAsGif(profilePictureFrames);
        return new List<MagickImage> { RenderAsPng(profilePictureFrames[0]) };
    }

    private static List<MagickImage> RenderAsGif(List<IMagickImage<byte>> profilePictureFrames)
    {
        var result = new List<MagickImage>();

        foreach (var frame in profilePictureFrames)
        {
            frame.Resize(180, 180);
            frame.RoundImage();

            var newFrame = RenderFrame(frame);

            newFrame.AnimationDelay = frame.AnimationDelay;
            newFrame.GifDisposeMethod = GifDisposeMethod.Background;
            result.Add(newFrame);
        }

        return result;
    }

    private static MagickImage RenderAsPng(IMagickImage<byte> profilePictureFrame)
    {
        profilePictureFrame.Resize(180, 180);
        profilePictureFrame.RoundImage();

        return RenderFrame(profilePictureFrame);
    }

    private static MagickImage RenderFrame(IMagickImage<byte> profilePictureFrame)
    {
        var bodyResource = PeepoloveResources.GetBody();
        using var body = bodyResource.Clone();

        new Drawables()
            .Composite(5, 312, CompositeOperator.Over, profilePictureFrame)
            .Composite(0, 0, CompositeOperator.Over, PeepoloveResources.GetHands())
            .Draw(body);

        body.Crop(new MagickGeometry(0, 115, 512, 397));
        return new MagickImage(body);
    }
}
