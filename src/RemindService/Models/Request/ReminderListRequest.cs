using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;
using GrillBot.Core.Validation;
using RemindService.Validators;
using System.ComponentModel.DataAnnotations;

namespace RemindService.Models.Request;

public class ReminderListRequest : IValidatableObject
{
    [DiscordId]
    [StringLength(32)]
    public string? FromUserId { get; set; }

    [DiscordId]
    [StringLength(32)]
    public string? ToUserId { get; set; }

    [DiscordId]
    [StringLength(32)]
    public string? CommandMessageId { get; set; }

    public string? MessageContains { get; set; }

    public DateTime? NotifyAtFromUtc { get; set; }
    public DateTime? NotifyAtToUtc { get; set; }

    public bool? OnlyPending { get; set; }
    public bool? OnlyInProcess { get; set; }

    public SortParameters Sort { get; set; } = new() { OrderBy = "Id" };
    public PaginatedParams Pagination { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        => new ReminderListRequestValidator().Validate(this, validationContext);
}
