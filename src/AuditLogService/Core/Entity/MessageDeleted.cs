using System.ComponentModel.DataAnnotations;

namespace AuditLogService.Core.Entity;

public class MessageDeleted : ChildEntityBase
{
    [StringLength(32)]
    public string AuthorId { get; set; } = null!;
    
    public DateTime MessageCreatedAt { get; set; }

    public string? Content { get; set; }
    
    public ISet<EmbedInfo> Embeds { get; set; }

    public MessageDeleted()
    {
        Embeds = new HashSet<EmbedInfo>();
    }
}
