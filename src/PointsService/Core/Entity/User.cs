using System.ComponentModel.DataAnnotations;

namespace PointsService.Core.Entity;

public class User
{
    [StringLength(30)]
    public string Id { get; set; } = null!;

    [StringLength(30)]
    public string GuildId { get; set; } = null!;
    public bool PointsDisabled { get; set; }
    public DateTime? LastReactionIncrement { get; set; }
    public DateTime? LastMessageIncrement { get; set; }
    public DateTime? LastSuperReactionIncrement { get; set; }
    public bool IsUser { get; set; }
    public int PointsPosition { get; set; }
    public int ActivePoints { get; set; }
    public int ExpiredPoints { get; set; }
    public long MergedPoints { get; set; }
}
