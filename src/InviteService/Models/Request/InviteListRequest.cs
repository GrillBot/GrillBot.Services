using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;
using GrillBot.Core.Validation;
using System.ComponentModel.DataAnnotations;

namespace InviteService.Models.Request;

public class InviteListRequest : IValidatableObject
{
    [DiscordId]
    [StringLength(32)]
    public string? GuildId { get; set; }

    [DiscordId]
    [StringLength(32)]
    public string? CreatorId { get; set; }

    public bool OnlyWithoutCreator { get; set; }

    public string? Code { get; set; }

    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }

    /// <summary>
    /// Available: Code, Created
    /// Default: Code
    /// </summary>
    public SortParameters Sort { get; set; } = new() { OrderBy = "Code" };

    public PaginatedParams Pagination { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!string.IsNullOrEmpty(CreatorId) && OnlyWithoutCreator)
            yield return new("Cannot combine CreatorId and OnlyWithoutCreator properties.", [nameof(CreatorId), nameof(OnlyWithoutCreator)]);
    }
}
