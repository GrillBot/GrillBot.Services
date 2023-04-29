using System.ComponentModel.DataAnnotations;

namespace ImageProcessingService.Core.GraphicsService.Models.Chart;

public class ChartRequestData
{
    [Required]
    public ChartOptions Options { get; set; } = null!;

    [Required]
    public ChartData Data { get; set; } = null!;

    public IEnumerable<object> GetAtomicValues()
    {
        foreach (var value in Options.GetAtomicValues())
            yield return value;

        foreach (var value in Data.GetAtomicValues())
            yield return value;
    }
}
