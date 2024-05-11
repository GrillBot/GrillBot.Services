﻿using ImageMagick;

namespace ImageProcessingService.Resources;

public static class PointsResources
{
    public static readonly ImageResource _trophy
        = new("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");

    public static MagickImage GetTrophy() => _trophy.GetImage();
}
