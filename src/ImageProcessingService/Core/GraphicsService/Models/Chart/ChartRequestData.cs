using System.ComponentModel.DataAnnotations;

namespace ImageProcessingService.Core.GraphicsService.Models.Chart;

public class ChartRequestData
{
    [Required]
    public ChartOptions Options { get; set; } = null!;
    
    [Required]
    public ChartData Data { get; set; } = null!;
}
