using ImageMagick;
using ImageMagick.Drawing;
using ImageProcessingService.Extensions;
using ImageProcessingService.Resources;

namespace ImageProcessingService.Renderers;

public static class PeepoangryRenderer
{
    public static List<MagickImage> Render(List<IMagickImage<byte>> profilePictureFrames)
    {
        if (profilePictureFrames.Count == 0)
            throw new ArgumentException("Missing profile pictures.", nameof(profilePictureFrames));

        if (profilePictureFrames.Count > 1)
            return RenderAsGif(profilePictureFrames);
        return [RenderAsPng(profilePictureFrames[0])];
    }

    private static List<MagickImage> RenderAsGif(List<IMagickImage<byte>> profilePictureFrames)
    {
        var result = new List<MagickImage>();

        foreach (var frame in profilePictureFrames)
        {
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
        profilePictureFrame.RoundImage();

        return RenderFrame(profilePictureFrame);
    }

    private static MagickImage RenderFrame(IMagickImage<byte> profilePictureFrame)
    {
        var layer = new MagickImage(MagickColors.Transparent, 250, 105);
        var peepoResource = PeepoangryResources.GetPeepo();

        new Drawables()
            .Composite(20, 10, CompositeOperator.Over, profilePictureFrame)
            .Composite(115, -5, CompositeOperator.Over, peepoResource)
            .Draw(layer);

        return layer;
    }
}
