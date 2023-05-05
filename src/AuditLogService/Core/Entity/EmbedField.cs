using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditLogService.Core.Entity;

public class EmbedField
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    public Guid EmbedInfoId { get; set; }

    public EmbedInfo EmbedInfo { get; set; } = null!;

    [StringLength(256)]
    public string Name { get; set; } = null!;

    [StringLength(1024)]
    public string Value { get; set; } = null!;

    public bool Inline { get; set; }
}
