using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Core.Entity;

public class ChildEntityBase
{
    [Key]
    public Guid LogItemId { get; set; }

    public LogItem LogItem { get; set; } = null!;
}
