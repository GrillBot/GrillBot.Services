using System.ComponentModel.DataAnnotations;

namespace PointsService.Core.Entity;

public class Channel
{
    [Required]
    [StringLength(30)]
    public string Id { get; set; } = null!;

    [Required]
    [StringLength(30)]
    public string GuildId { get; set; } = null!;
    
    public bool IsDeleted { get; set; }
    
    public bool PointsDisabled { get; set; }
}
