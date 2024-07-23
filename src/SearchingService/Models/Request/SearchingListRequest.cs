using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;
using GrillBot.Core.Validation;
using SearchingService.Validators;
using System.ComponentModel.DataAnnotations;

namespace SearchingService.Models.Request;

public class SearchingListRequest : IValidatableObject
{
    [DiscordId]
    [StringLength(30)]
    public string? UserId { get; set; }

    [DiscordId]
    [StringLength(30)]
    public string? GuildId { get; set; }

    [DiscordId]
    [StringLength(30)]
    public string? ChannelId { get; set; }

    public string? MessageQuery { get; set; }

    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }

    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public bool HideInvalid { get; set; }
    public bool ShowDeleted { get; set; }

    /// <summary>
    /// Available: Id, CreatedAt, ValidTo
    /// Default: Id
    /// </summary>
    public SortParameters Sort { get; set; } = new() { OrderBy = "Id" };

    public PaginatedParams Pagination { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        => new SearchingListRequestValidator().Validate(this, validationContext);
}
