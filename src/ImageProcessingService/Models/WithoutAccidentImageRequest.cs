using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;

namespace ImageProcessingService.Models;

public class WithoutAccidentImageRequest
{
    [StringLength(30)]
    [DiscordId]
    public string UserId { get; set; } = null!;

    [Required]
    [Range(0, int.MaxValue)]
    public int DaysCount { get; set; }

    [Required]
    public AvatarInfo AvatarInfo { get; set; } = null!;
}
