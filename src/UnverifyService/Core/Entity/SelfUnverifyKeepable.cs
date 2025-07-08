using System.ComponentModel.DataAnnotations;

namespace UnverifyService.Core.Entity;

public class SelfUnverifyKeepable
{
    [StringLength(100)]
    public string Group { get; set; } = "";

    [StringLength(100)]
    public string Name { get; set; } = "";

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
