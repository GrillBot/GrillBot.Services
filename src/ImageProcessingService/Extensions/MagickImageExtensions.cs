using ImageMagick;

namespace ImageProcessingService.Extensions;

public static class MagickImageExtensions
{
    public static void RoundImage(this IMagickImage<byte> image)
    {
        image.Format = MagickFormat.Png;
        image.Alpha(AlphaOption.On);

        using var copy = image.Clone();

        copy.Distort(DistortMethod.DePolar, 0);
        copy.VirtualPixelMethod = VirtualPixelMethod.HorizontalTile;
        copy.BackgroundColor = MagickColors.None;
        copy.Distort(DistortMethod.Polar, 0);

        image.Compose = CompositeOperator.DstIn;
        image.Composite(copy, CompositeOperator.CopyAlpha);
    }
}
