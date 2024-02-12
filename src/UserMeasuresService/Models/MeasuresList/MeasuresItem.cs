namespace UserMeasuresService.Models.MeasuresList;

public class MeasuresItem
{
    public string Type { get; set; } = null!;
    public DateTime CreatedAtUtc { get; set; }
    public string ModeratorId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string GuildId { get; set; } = null!;
    public DateTime? ValidTo { get; set; }
}
