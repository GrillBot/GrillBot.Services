using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;

namespace PointsService.Models.Users;

public class UserInfo
{
    [Required]
    [StringLength(30)]
    [DiscordId]
    public string Id { get; set; } = null!;

    [Required]
    public bool PointsDisabled { get; set; }

    [Required]
    public bool IsUser { get; set; }
}
