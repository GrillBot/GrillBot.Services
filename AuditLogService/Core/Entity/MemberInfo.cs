using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditLogService.Core.Entity;

public class MemberInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    [StringLength(32)]
    public string UserId { get; set; } = null!;

    [StringLength(32)]
    public string? Nickname { get; set; }

    public bool? IsMuted { get; set; }
    public bool? IsDeaf { get; set; }
    public string? SelfUnverifyMinimalTime { get; set; }
    public int? Flags { get; set; }
}
