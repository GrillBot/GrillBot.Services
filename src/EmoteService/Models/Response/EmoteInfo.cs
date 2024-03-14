namespace EmoteService.Models.Response;

public class EmoteInfo
{
    public string EmoteName { get; set; } = null!;
    public bool IsEmoteAnimated { get; set; }
    public string EmoteUrl { get; set; } = null!;
    public string? OwnerGuildId { get; set; }
    public EmoteInfoStatistics? Statistics { get; set; }
}
