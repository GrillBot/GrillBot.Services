namespace PointsService.Core.Options;

public class AppOptions
{
    public int MinimalMessageLength { get; set; }
    public int ReactionCooldown { get; set; }
    public int MessageCooldown { get; set; }
    public int ReactionPointsMin { get; set; }
    public int ReactionPointsMax { get; set; }
    public int MessagePointsMin { get; set; }
    public int MessagePointsMax { get; set; }
    public int MinimalTransactionsForMerge { get; set; }
    public int ExpirationMonths { get; set; }
}
