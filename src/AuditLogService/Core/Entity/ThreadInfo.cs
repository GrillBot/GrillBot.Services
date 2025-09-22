using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Discord;

namespace AuditLogService.Core.Entity;

public class ThreadInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    public string ThreadName { get; set; } = null!;
    public int? SlowMode { get; set; }
    public ThreadType Type { get; set; }
    public bool IsArchived { get; set; }
    public ThreadArchiveDuration ArchiveDuration { get; set; }
    public bool IsLocked { get; set; }

    [Column(TypeName = "jsonb")]
    public List<string>? Tags { get; set; }
}
