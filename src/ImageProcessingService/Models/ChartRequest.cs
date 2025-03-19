using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Services.Graphics.Models.Chart;
using GrillBot.Core.Validation;

namespace ImageProcessingService.Models;

public class ChartRequest
{
    [Required]
    [RequireSomeItemInCollection(ErrorMessage = "Some request is required.")]
    public List<ChartRequestData> Requests { get; set; } = [];
}
