namespace EmoteService.Models.Response;

public class EmoteUserUsageItem
{
    public string UserId { get; set; } = null!;
    public long UseCount { get; set; }
    public DateTime FirstOccurence { get; set; }
    public DateTime LastOccurence { get; set; }
}
