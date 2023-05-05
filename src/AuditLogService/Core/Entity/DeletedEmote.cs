using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Core.Entity;

public class DeletedEmote : ChildEntityBase
{
    [StringLength(32)]
    public string EmoteId { get; set; } = null!;

    public string EmoteName { get; set; } = null!;
}
