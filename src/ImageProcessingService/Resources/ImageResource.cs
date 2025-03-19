using ImageMagick;

namespace ImageProcessingService.Resources;

public class ImageResource(string base64)
{
    private MagickImage? _image;

    public MagickImage GetImage()
    {
        _image ??= new MagickImage(Convert.FromBase64String(base64));
        return _image;
    }
}
