﻿using ImageMagick;
using ImageMagick.Drawing;
using ImageProcessingService.Extensions;
using ImageProcessingService.Resources;
using System.Drawing;

namespace ImageProcessingService.Renderers;

public static class PointsImageRenderer
{
    private static readonly Size _size = new(512, 410);
    private static readonly (string name, int size) _usernameFont = ("Open Sans", 70);
    private static readonly (string name, int size) _positionFont = ("Open Sans", 35);
    private static readonly (string name, int size) _pointsFont = ("Open Sans", 40);
    private const uint PROFILE_PICTURE_SIZE = 200;
    private const int BORDER = 25;
    private const int CORDER_RADIUS = 20;

    public static MagickImage Render(IMagickImage<byte> profilePicture, string username, int points, int position)
    {
        var dominantColor = profilePicture.GetDominantColor();
        var textBackground = CreateDarkerBackgroundColor(dominantColor);
        var disposables = new List<IDisposable>(); // Objects for release after drawing.
        var image = new MagickImage(dominantColor, (uint)_size.Width, (uint)_size.Height);

        try
        {
            var drawables = new Drawables()
                .EnableStrokeAntialias()
                .EnableTextAntialias()
                .FillColor(textBackground)
                .RoundRectangle(BORDER, BORDER, _size.Width - BORDER, _size.Height - BORDER, CORDER_RADIUS, CORDER_RADIUS);

            SetProfilePicture(drawables, profilePicture, disposables);
            SetUsername(drawables, username);
            SetPointsStatus(drawables, points, position);

            drawables.Draw(image);
        }
        finally
        {
            foreach (var disposable in disposables)
                disposable.Dispose();
        }

        return image;
    }

    private static MagickColor CreateDarkerBackgroundColor(IMagickColor<byte> color)
    {
        var tmpColor = Color.FromArgb(color.A, color.R, color.G, color.B);
        tmpColor = tmpColor.GetBrightness() <= 0.2 ? Color.FromArgb(25, Color.White) : Color.FromArgb(100, Color.Black);

        return MagickColor.FromRgba(tmpColor.R, tmpColor.G, tmpColor.B, tmpColor.A);
    }

    private static void SetProfilePicture(IDrawables<byte> drawables, IMagickImage<byte> profilePicture, List<IDisposable> disposables)
    {
        profilePicture.Resize(PROFILE_PICTURE_SIZE, PROFILE_PICTURE_SIZE);
        profilePicture.RoundCorners(CORDER_RADIUS);

        var shadow = profilePicture.Clone();
        shadow.Shadow(0, 0, 5, new(90), MagickColors.Black);
        shadow.BackgroundColor = MagickColors.None;
        disposables.Add(shadow);

        drawables
            .Composite(BORDER + 10, BORDER + 10, CompositeOperator.Over, shadow)
            .Composite(BORDER + 10, BORDER + 10, CompositeOperator.Over, profilePicture);
    }

    private static void SetUsername(IDrawables<byte> drawables, string username)
    {
        var maxLength = _size.Width - ((10 + BORDER) * 2);
        var printableUsername = username.CutToImageWidth(maxLength, _usernameFont.name, _usernameFont.size);
        var textPosition = BORDER + 20 + PROFILE_PICTURE_SIZE + _usernameFont.size;

        drawables
            .Font(_usernameFont.name)
            .FontPointSize(_usernameFont.size)
            .TextAlignment(TextAlignment.Left)
            .FillColor(MagickColors.White)
            .Text(BORDER + 10, textPosition, printableUsername);
    }

    private static void SetPointsStatus(IDrawables<byte> drawables, int points, int position)
    {
        var positionText = $"{position}. místo";
        var positionTextSize = positionText.MeasureText(_positionFont.name, _positionFont.size);
        var positionTextX = Math.Max(
            _size.Width - BORDER - 10 - positionTextSize.Width, // Expected position
            BORDER + 15 + PROFILE_PICTURE_SIZE // Start space.
        );
        var positionTextY = BORDER + 5 + _positionFont.size;

        // Position
        drawables
            .Font(_positionFont.name)
            .FontPointSize(_positionFont.size)
            .TextAlignment(TextAlignment.Left)
            .FillColor(MagickColors.White)
            .Text(positionTextX, positionTextY, positionText);

        // Points
        drawables
            .Font(_pointsFont.name)
            .FontPointSize(_pointsFont.size)
            .TextAlignment(TextAlignment.Left)
            .FillColor(MagickColors.White)
            .Text(
                BORDER + 15,
                _size.Height - BORDER - 15,
                (points switch
                {
                    1 => "1 bod",
                    > 1 and < 5 => $"{points} body",
                    _ => $"{points.ToStringWithSpaces()} bodů"
                }).CutToImageWidth(_size.Width - ((10 + BORDER) * 2), _pointsFont.name, _pointsFont.size)
            );

        // Trophy
        if (position == 1)
        {
            var trophy = PointsResources.GetTrophy();

            drawables.Composite(
                _size.Width - BORDER - 10 - trophy.Width,
                BORDER + 15 + _positionFont.size,
                CompositeOperator.Over,
                trophy
            );
        }
    }
}
