using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;
using ImageMagick;

namespace ImageProcessingService.Models;

public class PeepoRequest
{
    /// <summary>
    /// Guild upload limit in bytes.
    /// </summary>
    [Required]
    [Range(0, long.MaxValue)]
    public long GuildUploadLimit { get; set; }

    [Required]
    [StringLength(30)]
    [DiscordId]
    public string UserId { get; set; } = null!;

    [Required]
    public AvatarInfo AvatarInfo { get; set; } = null!;

    public bool IsAnimated()
    {
        if (AvatarInfo.Type == "png")
            return false;

        var avatarSize = AvatarInfo.AvatarContent.Length;
        return avatarSize <= 2 * (GuildUploadLimit / 1000000 * 1024 * 1024 / 3);
    }

    public List<IMagickImage<byte>> GetProfilePictureFrames()
    {
        using var profilePicture = new MagickImageCollection(AvatarInfo.AvatarContent);

        if (IsAnimated())
        {
            profilePicture.Coalesce();
            return profilePicture.Select(o => o.Clone()).ToList();
        }
        else
        {
            return new List<IMagickImage<byte>> { profilePicture[0].Clone() };
        }
    }
}
