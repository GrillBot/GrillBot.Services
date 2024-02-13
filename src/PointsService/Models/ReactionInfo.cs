namespace PointsService.Models;

public class ReactionInfo
{
    /// <summary>
    /// The user who added the reaction.
    /// </summary>
    public string UserId { get; set; } = null!;
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
