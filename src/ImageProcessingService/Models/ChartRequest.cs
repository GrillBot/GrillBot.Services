using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;
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

    public string GetHash()
    {
        var builder = new StringBuilder();

        foreach (var item in Requests.SelectMany(o => o.GetAtomicValues()))
            builder.Append($"|{item}|");

        var hashData = builder.ToString();
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(hashData));
        return Convert.ToBase64String(hash);
    }
}
