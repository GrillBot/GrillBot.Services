using ImageMagick;

namespace ImageProcessingService.Resources;

public class ImageResource
{
    private MagickImage? _image;
    private readonly string _base64;

    public ImageResource(string base64)
    {
        _base64 = base64;
    }

    public MagickImage GetImage()
    {
        _image ??= new MagickImage(Convert.FromBase64String(_base64));
        return _image;
    }
}
