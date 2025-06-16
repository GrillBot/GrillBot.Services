using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace MessageService.Core.Entity;

public class AutoReplyDefinition
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Template { get; set; } = null!;
    public string Reply { get; set; } = null!;
    public bool IsDeleted { get; set; }
    public bool IsDisabled { get; set; }
    public bool IsCaseSensitive { get; set; }

    [NotMapped]
    public RegexOptions RegexOptions
        => RegexOptions.Singleline | (IsCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
}
