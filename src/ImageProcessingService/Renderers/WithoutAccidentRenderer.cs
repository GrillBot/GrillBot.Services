using ImageMagick;
using ImageMagick.Drawing;
using ImageProcessingService.Extensions;
using ImageProcessingService.Resources;
using System.Drawing;

namespace ImageProcessingService.Renderers;

public static class WithoutAccidentRenderer
{
    private const string FONT_NAME = "Open Sans";

    public static MagickImage Render(IMagickImage<byte> profilePicture, int daysCount)
    {
        var background = WithoutAccidentResources.GetBackground();
        var image = new MagickImage(background);

        var drawables = new Drawables()
            .EnableStrokeAntialias()
            .EnableTextAntialias();

        SetDaysCount(drawables, daysCount);
        SetProfilePicture(drawables, profilePicture);
        drawables.Composite(0, 0, CompositeOperator.Over, WithoutAccidentResources.GetHead());
        drawables.Composite(0, 0, CompositeOperator.Over, WithoutAccidentResources.GetPliers());

        drawables.Draw(image);
        return image;
    }

    private static void SetDaysCount(IDrawables<byte> drawables, int daysCount)
    {
        var fontSize = daysCount switch
        {
            < 10 => 100,
            >= 10 and < 100 => 70,
            >= 100 and < 1000 => 50,
            _ => 40
        };

        var coordinates = daysCount switch
        {
            < 10 => new Point(1060, 280),
            >= 10 and < 100 => new Point(1053, 270),
            >= 100 and < 1000 => new Point(1048, 250),
            _ => new Point(1043, 260)
        };

        drawables
            .Font(FONT_NAME)
            .FontPointSize(fontSize)
            .TextAlignment(TextAlignment.Left)
            .Text(coordinates.X, coordinates.Y, daysCount.ToString());
    }

    private static void SetProfilePicture(IDrawables<byte> drawables, IMagickImage<byte> profilePicture)
    {
        profilePicture.Resize(230, 230);
        profilePicture.RoundImage();
        profilePicture.Crop(230, 200);

        drawables
            .Rotation(-6)
            .Composite(510, 335, CompositeOperator.Over, profilePicture)
            .Rotation(6);
    }
}
