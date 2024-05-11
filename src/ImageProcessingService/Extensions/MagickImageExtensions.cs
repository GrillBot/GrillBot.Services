using ImageMagick;
using System.Drawing;
using System.Text;

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

    public static void RoundCorners(this IMagickImage<byte> image, int radius)
    {
        using var mask = new MagickImage(MagickColors.Transparent, image.Width, image.Height);
        new Drawables().RoundRectangle(0, 0, image.Width, image.Height, radius, radius).Draw(mask);

        image.Composite(mask, CompositeOperator.CopyAlpha);
    }

    public static IMagickColor<byte> GetDominantColor(this IMagickImage<byte> image)
    {
        using var clone = image.Clone();
        clone.HasAlpha = false;

        var histogram = clone.Histogram();
        var histogramValue = histogram.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
        return new MagickColor(histogramValue);
    }

    public static string CutToImageWidth(this string value, int maxWidth, string font, int fontSize)
    {
        var drawables = new Drawables().Font(font).FontPointSize(fontSize);
        var builder = new StringBuilder();

        foreach (var character in value)
        {
            var metrics = drawables.FontTypeMetrics(builder.ToString() + character);
            if (metrics is null) continue;

            if (metrics.TextWidth >= maxWidth) break;
            builder.Append(character);
        }

        var result = builder.ToString();
        return result != value ? result[..^3] + "..." : result;
    }

    public static SizeF MeasureText(this string value, string font, int fontSize)
    {
        var drawables = new Drawables().Font(font).FontPointSize(fontSize);
        var size = drawables.FontTypeMetrics(value);

        return size is null ?
            new SizeF(0, 0) :
            new SizeF(Convert.ToSingle(size.TextWidth), Convert.ToSingle(size.TextHeight));
    }
}
