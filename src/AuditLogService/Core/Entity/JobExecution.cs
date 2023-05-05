using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Core.Entity;

public class JobExecution : ChildEntityBase
{
    [StringLength(128)]
    public string JobName { get; set; } = null!;

    public string Result { get; set; } = null!;

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }

    public bool WasError { get; set; }

    [StringLength(32)]
    public string? StartUserId { get; set; }
}
