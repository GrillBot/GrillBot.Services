namespace EmoteService.Models.Response;

public class EmoteStatisticsItem
{
    public string GuildId { get; set; } = null!;
    public string EmoteId { get; set; } = null!;
    public string EmoteName { get; set; } = null!;
    public bool EmoteIsAnimated { get; set; }
    public string? EmoteUrl { get; set; }
    public long UseCount { get; set; }
    public int UsersCount { get; set; }
    public DateTime FirstOccurence { get; set; }
    public DateTime LastOccurence { get; set; }
}
