using System.ComponentModel.DataAnnotations;
using ImageProcessingService.Core.GraphicsService.Models.Chart;

namespace ImageProcessingService.Models;

public class ChartRequest : IValidatableObject
{
    [Required]
    public List<ChartRequestData> Requests { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Requests.Count == 0)
            yield return new ValidationResult("Some request is required.");
    }
}
