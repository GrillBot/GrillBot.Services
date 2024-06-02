using Discord;
using GrillBot.Core.Validation;
using RemindService.Validators;
using System.ComponentModel.DataAnnotations;

namespace RemindService.Models.Request;

public class CreateReminderRequest : IValidatableObject
{
    [DiscordId]
    [StringLength(32)]
    public string FromUserId { get; set; } = null!;

    [DiscordId]
    [StringLength(32)]
    public string ToUserId { get; set; } = null!;

    public DateTime NotifyAtUtc { get; set; }

    [Required(ErrorMessage = "RemindModule/Create/Validation/MessageRequired")]
    [StringLength(EmbedFieldBuilder.MaxFieldValueLength, ErrorMessage = "RemindModule/Create/Validation/MaxLengthExceeded")]
    public string Message { get; set; } = null!;

    [DiscordId]
    [StringLength(32)]
    public string CommandMessageId { get; set; } = null!;

    [StringLength(32)]
    public string Language { get; set; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        => new CreateReminderRequestValidator().Validate(this, validationContext);
}
