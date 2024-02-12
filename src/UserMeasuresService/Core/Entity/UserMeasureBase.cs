using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserMeasuresService.Core.Entity;

public abstract class UserMeasureBase
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    [StringLength(32)]
    public string ModeratorId { get; set; } = null!;

    [StringLength(32)]
    public string GuildId { get; set; } = null!;

    [StringLength(32)]
    public string UserId { get; set; } = null!;

    public string Reason { get; set; } = null!;
}
