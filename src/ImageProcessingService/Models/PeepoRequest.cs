using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;
using ImageMagick;
using ImageProcessingService.Extensions;

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
        => AvatarInfo.IsAnimated(GuildUploadLimit);

    public List<IMagickImage<byte>> GetProfilePictureFrames()
        => AvatarInfo.GetProfilePictureFrames(GuildUploadLimit).ToList();
}
