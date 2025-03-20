using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;
using GrillBot.Core.Validation;

namespace PointsService.Models;

public class AdminListRequest : IValidatableObject
{
    [Required]
    public bool ShowMerged { get; set; }

    [StringLength(30)]
    [DiscordId]
    public string? GuildId { get; set; }

    [StringLength(30)]
    [DiscordId]
    public string? UserId { get; set; }

    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }

    public bool OnlyReactions { get; set; }
    public bool OnlyMessages { get; set; }

    [DiscordId]
    [StringLength(30)]
    public string? MessageId { get; set; }

    public PaginatedParams Pagination { get; set; } = new();
    public SortParameters? Sort { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CreatedFrom > CreatedTo)
            yield return new ValidationResult("Invalid interval From-To", [nameof(CreatedFrom), nameof(CreatedTo)]);
    }
}
