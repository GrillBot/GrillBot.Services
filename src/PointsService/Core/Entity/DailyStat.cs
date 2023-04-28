using System.ComponentModel.DataAnnotations;

namespace PointsService.Core.Entity;

public class DailyStat
{
    [Required]
    [StringLength(30)]
    public string GuildId { get; set; } = null!;

    [Required]
    [StringLength(30)]
    public string UserId { get; set; } = null!;
    
    [Required]
    public DateOnly Date { get; set; }
    
    public long MessagePoints { get; set; }
    public long ReactionPoints { get; set; }
}
