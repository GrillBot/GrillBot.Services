using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;

namespace PointsService.Models;

public class ChannelInfo
{
    [Required]
    [StringLength(30)]
    [DiscordId]
    public string Id { get; set; } = null!;

    [Required]
    public bool IsDeleted { get; set; }

    [Required]
    public bool PointsDisabled { get; set; }
}
