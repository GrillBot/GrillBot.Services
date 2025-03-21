using GrillBot.Core.Models.Pagination;
using GrillBot.Core.Validation;
using System.ComponentModel.DataAnnotations;

namespace UserMeasuresService.Models.Measures;

public class MeasuresListParams : IValidatableObject
{
    /// <summary>
    /// Possible values: Unverify, Timeout, Warning
    /// </summary>
    [StringLength(10)]
    public string? Type { get; set; }

    [DiscordId]
    public string? GuildId { get; set; }

    [DiscordId]
    public string? UserId { get; set; }

    [DiscordId]
    public string? ModeratorId { get; set; }

    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public PaginatedParams Pagination { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var possibleTypes = new[] { "Unverify", "Timeout", "Warning" };
        if (!string.IsNullOrEmpty(Type) && !possibleTypes.Contains(Type))
            yield return new ValidationResult("Unsupported type.", [nameof(Type)]);
    }
}
