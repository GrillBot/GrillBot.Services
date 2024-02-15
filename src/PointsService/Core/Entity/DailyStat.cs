using System.ComponentModel.DataAnnotations;

namespace PointsService.Core.Entity;

public class DailyStat
{
    [StringLength(30)]
    public string GuildId { get; set; } = null!;

    [StringLength(30)]
    public string UserId { get; set; } = null!;

    public DateOnly Date { get; set; }
    public long MessagePoints { get; set; }
    public long ReactionPoints { get; set; }
}
