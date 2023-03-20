using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;
using PointsService.Validation;

namespace PointsService.Models;

public class TransactionRequest : IValidatableObject
{
    [Required]
    [StringLength(30)]
    [DiscordId]
    public string GuildId { get; set; } = null!;

    [Required]
    [StringLength(30)]
    [DiscordId]
    public string ChannelId { get; set; } = null!;

    [Required]
    public MessageInfo MessageInfo { get; set; } = null!;

    public ReactionInfo? ReactionInfo { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        => validationContext.GetRequiredService<TransactionRequestValidator>().Validate(this);
}
