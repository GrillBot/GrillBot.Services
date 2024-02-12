using GrillBot.Core.Models.Pagination;
using Microsoft.AspNetCore.Mvc;
using UserMeasuresService.Actions.MeasuresList;
using UserMeasuresService.Models.MeasuresList;

namespace UserMeasuresService.Controllers;

[Route("api/list")]
public class MeasuresListController : GrillBot.Core.Infrastructure.Actions.ControllerBase
{
    public MeasuresListController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost]
    [ProducesResponseType(typeof(PaginatedResponse<MeasuresItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMeasuresListAsync(MeasuresListParams parameters)
        => await ProcessAsync<GetMeasuresList>(parameters);
}
