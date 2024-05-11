using ImageMagick;
using ImageProcessingService.Models;

namespace ImageProcessingService.Extensions;

public static class AvatarInfoExtensions
{
    public static IEnumerable<IMagickImage<byte>> GetProfilePictureFrames(this AvatarInfo avatar, long guildUploadLimit)
    {
        using var profilePicture = new MagickImageCollection(avatar.AvatarContent);

        if (IsAnimated(avatar, guildUploadLimit))
        {
            profilePicture.Coalesce();

            foreach (var frame in profilePicture.Select(o => o.Clone()))
                yield return frame;
        }
        else
        {
            yield return profilePicture[0].Clone();
        }
    }

    public static bool IsAnimated(this AvatarInfo avatar, long guildUploadLimit)
    {
        if (avatar.Type == "png")
            return false;

        return avatar.AvatarContent.Length <= 2 * (guildUploadLimit / 1000000 * 1024 * 1024 / 3);
    }
}
