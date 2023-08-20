using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Services.Graphics.Models.Chart;

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
