using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;

namespace PointsService.Models;

public class UserInfo
{
    [Required]
    [StringLength(30)]
    [DiscordId]
    public string Id { get; set; } = null!;

    [Required]
    public bool IsUser { get; set; }
}
