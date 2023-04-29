using System.ComponentModel.DataAnnotations;

namespace ImageProcessingService.Models;

public class AvatarInfo : IValidatableObject
{
    [Required]
    public string AvatarId { get; set; } = null!;

    [Required]
    public byte[] AvatarContent { get; set; } = null!;

    [Required]
    public string Type { get; set; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Type != "gif" && Type != "png")
            yield return new ValidationResult("Only gifs and png avatars are allowed.", new[] { nameof(Type) });
    }
}
