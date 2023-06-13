using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditLogService.Core.Entity;

public class File
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    public Guid LogItemId { get; set; }

    public LogItem LogItem { get; set; } = null!;

    [StringLength(255)]
    public string Filename { get; set; } = null!;

    [StringLength(255)]
    public string? Extension { get; set; }

    public long Size { get; set; }
}
