using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;

namespace ImageProcessingService.Models;

public class PointsRequest
{
    [Required]
    [StringLength(30)]
    [DiscordId]
    public string UserId { get; set; } = null!;

    [Required]
    [StringLength(32, MinimumLength = 2)]
    public string Username { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue)]
    public int PointsValue { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int Position { get; set; }

    [Required]
    public AvatarInfo AvatarInfo { get; set; } = null!;
}
