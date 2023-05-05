using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditLogService.Core.Entity;

public class MemberRoleUpdated
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    public Guid LogItemId { get; set; }
    public LogItem LogItem { get; set; } = null!;

    [StringLength(32)]
    public string UserId { get; set; } = null!;

    [StringLength(32)]
    public string RoleId { get; set; } = null!;

    public string RoleName { get; set; } = null!;

    [StringLength(32)]
    public string RoleColor { get; set; } = null!;

    public bool IsAdded { get; set; }
}
