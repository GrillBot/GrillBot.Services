using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditLogService.Core.Entity;

public class RoleInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    [StringLength(32)]
    public string RoleId { get; set; } = null!;

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(32)]
    public string? Color { get; set; }

    public bool IsMentionable { get; set; }
    public bool IsHoisted { get; set; }

    [Column(TypeName = "jsonb")]
    public List<string> Permissions { get; set; } = [];

    [StringLength(256)]
    public string? IconId { get; set; }
}
