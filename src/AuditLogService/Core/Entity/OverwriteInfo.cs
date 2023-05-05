using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Discord;

namespace AuditLogService.Core.Entity;

public class OverwriteInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }
    
    public PermissionTarget Target { get; set; }

    [StringLength(32)]
    public string TargetId { get; set; } = null!;

    public string AllowValue { get; set; } = "0";
    public string DenyValue { get; set; } = "0";
}
