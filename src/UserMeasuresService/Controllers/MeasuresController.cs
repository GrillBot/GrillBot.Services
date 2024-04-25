using GrillBot.Core.Models.Pagination;
using Microsoft.AspNetCore.Mvc;
using UserMeasuresService.Models.Measures;

namespace UserMeasuresService.Controllers;

public class MeasuresController : GrillBot.Core.Infrastructure.Actions.ControllerBase
{
    public MeasuresController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost("list")]
    [ProducesResponseType(typeof(PaginatedResponse<MeasuresItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetMeasuresListAsync(MeasuresListParams parameters)
        => ProcessAsync<Actions.Measures.GetMeasuresList>(parameters);

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> DeleteMeasureAsync(DeleteMeasuresRequest request)
        => ProcessAsync<Actions.Measures.DeleteMeasureAction>(request);
}
