using System.ComponentModel.DataAnnotations;

namespace EmoteService.Core.Entity.ValueObjects;

public class EmoteValueObject
{
    [StringLength(32)]
    public string EmoteId { get; set; } = null!;

    [StringLength(255)]
    public string EmoteName { get; set; } = null!;

    public bool EmoteIsAnimated { get; set; }

    public override string ToString()
        => $"<{(EmoteIsAnimated ? "a" : "")}:{EmoteName}:{EmoteId}>";
}
