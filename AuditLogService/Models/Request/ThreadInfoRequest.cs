using System.ComponentModel.DataAnnotations;
using Discord;

namespace AuditLogService.Models.Request;

public class ThreadInfoRequest
{
    [Required]
    public string ThreadName { get; set; } = null!;
    
    public int? SlowMode { get; set; }
    
    [Required]
    public ThreadType Type { get; set; }
    
    [Required]
    public bool IsArchived { get; set; }
    
    [Required]
    public int ArchiveDuration { get; set; }
    
    [Required]
    public bool IsLocked { get; set; }

    public List<string> Tags { get; set; } = new();
}
