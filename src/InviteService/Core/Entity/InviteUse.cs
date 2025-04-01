using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InviteService.Core.Entity;

public class InviteUse
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.NewGuid();

    [StringLength(32)]
    public string GuildId { get; set; } = null!;

    [StringLength(32)]
    public string UserId { get; set; } = null!;

    [StringLength(10)]
    public string Code { get; set; } = null!;

    public Invite Invite { get; set; } = null!;

    public DateTime UsedAt { get; set; }
}
