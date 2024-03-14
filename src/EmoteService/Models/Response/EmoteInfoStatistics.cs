namespace EmoteService.Models.Response;

public class EmoteInfoStatistics
{
    public DateTime FirstOccurenceUtc { get; set; }
    public DateTime LastOccurenceUtc { get; set; }
    public long UseCount { get; set; }
    public long UsersCount { get; set; }
    public Dictionary<string, long> TopUsers { get; set; } = new();
}
