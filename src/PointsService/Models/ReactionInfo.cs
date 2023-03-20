using System.ComponentModel.DataAnnotations;

namespace PointsService.Models;

public class ReactionInfo
{
    /// <summary>
    /// The user who added the reaction.
    /// </summary>
    [Required]
    public UserInfo User { get; set; } = null!;

    [Required]
    public string Emote { get; set; } = null!;
}
