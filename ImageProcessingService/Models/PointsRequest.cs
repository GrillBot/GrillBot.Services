using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;

namespace ImageProcessingService.Models;

public class PointsRequest : IValidatableObject
{
    [Required]
    [StringLength(30)]
    [DiscordId]
    public string UserId { get; set; } = null!;
    
    [Required]
    [StringLength(32, MinimumLength = 2)]
    public string Username { get; set; } = null!;

    [Required]
    public int PointsValue { get; set; }

    [Required]
    public int Position { get; set; }

    [Required]
    public AvatarInfo AvatarInfo { get; set; } = null!;


    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PointsValue < 0)
            yield return new ValidationResult("Points value cannot be negative.", new[] { nameof(PointsValue) });

        if (Position <= 0)
            yield return new ValidationResult("Position cannot be negative.", new[] { nameof(Position) });
    }
}
