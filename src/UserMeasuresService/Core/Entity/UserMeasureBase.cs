using System.ComponentModel.DataAnnotations;

namespace UserMeasuresService.Core.Entity;

public abstract class UserMeasureBase : BaseEntity
{
    public DateTime CreatedAtUtc { get; set; }

    [StringLength(32)]
    public string ModeratorId { get; set; } = null!;

    [StringLength(32)]
    public string GuildId { get; set; } = null!;

    [StringLength(32)]
    public string UserId { get; set; } = null!;

    public string Reason { get; set; } = null!;
}
