using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;

namespace PointsService.Models;

public class ReactionInfo
{
    /// <summary>
    /// The user who added the reaction.
    /// </summary>
    [Required]
    [StringLength(30)]
    [DiscordId]
    public string UserId { get; set; } = null!;

    [Required]
    public string Emote { get; set; } = null!;

    /// <summary>
    /// Is super reaction?
    /// </summary>
    public bool IsBurst { get; set; }

    public string GetReactionId()
    {
        var id = $"{UserId}_{Emote}";
        return IsBurst ? id + "_Burst" : id;
    }
}
