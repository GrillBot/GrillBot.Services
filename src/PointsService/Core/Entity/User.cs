using System.ComponentModel.DataAnnotations;

namespace PointsService.Core.Entity;

public class User
{
    [Required]
    [StringLength(30)]
    public string Id { get; set; } = null!;

    [Required]
    [StringLength(30)]
    public string GuildId { get; set; } = null!;

    public bool PointsDisabled { get; set; }

    public DateTime? LastReactionIncrement { get; set; }
    public DateTime? LastMessageIncrement { get; set; }

    public bool IsUser { get; set; }
    public int PointsPosition { get; set; }
    public bool PendingRecalculation { get; set; }
    public long ActivePoints { get; set; }
    public long ExpiredPoints { get; set; }
    public long MergedPoints { get; set; }
}
