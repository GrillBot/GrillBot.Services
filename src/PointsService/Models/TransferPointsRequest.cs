using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;

namespace PointsService.Models;

public class TransferPointsRequest : IValidatableObject
{
    [Required]
    [StringLength(30)]
    [DiscordId]
    public string GuildId { get; set; } = null!;

    [Required]
    [StringLength(30)]
    [DiscordId]
    public string FromUserId { get; set; } = null!;

    [Required]
    [StringLength(30)]
    [DiscordId]
    public string ToUserId { get; set; } = null!;

    [Required]
    [Range(1, int.MaxValue)]
    public int Amount { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (FromUserId == ToUserId)
            yield return new ValidationResult("Unable to transfer points between same accounts.", [nameof(FromUserId), nameof(ToUserId)]);
    }
}
