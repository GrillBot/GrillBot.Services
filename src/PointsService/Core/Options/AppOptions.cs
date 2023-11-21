namespace PointsService.Core.Options;

public class AppOptions
{
    public int MinimalTransactionsForMerge { get; set; }
    public int ExpirationMonths { get; set; }
    public IncrementOptions SuperReactions { get; set; } = null!;
    public IncrementOptions Reactions { get; set; } = null!;
    public IncrementOptions Message { get; set; } = null!;
}
