namespace UserMeasuresService.Models.Measures;

public class MeasuresItem
{
    public Guid MeasureId { get; set; }
    public string Type { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
    public string ModeratorId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string GuildId { get; set; } = null!;
    public DateTime? ValidTo { get; set; }
    public string Reason { get; set; } = null!;
}
