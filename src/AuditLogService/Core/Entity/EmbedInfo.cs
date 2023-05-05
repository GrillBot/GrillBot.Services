using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditLogService.Core.Entity;

public class EmbedInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    public Guid MessageDeletedId { get; set; }

    public MessageDeleted MessageDeleted { get; set; } = null!;

    [StringLength(256)]
    public string? Title { get; set; }

    [StringLength(16)]
    public string Type { get; set; } = null!;

    public string? ImageInfo { get; set; }
    public string? VideoInfo { get; set; }
    public string? AuthorName { get; set; }
    public bool ContainsFooter { get; set; }
    public string? ProviderName { get; set; }
    public string? ThumbnailInfo { get; set; }
    
    public ISet<EmbedField> Fields { get; set; }

    public EmbedInfo()
    {
        Fields = new HashSet<EmbedField>();
    }
}
