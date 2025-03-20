using Microsoft.AspNetCore.Mvc;
using PointsService.Actions;
using PointsService.Models;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace PointsService.Controllers;

public class ChartController(IServiceProvider serviceProvider) : ControllerBase(serviceProvider)
{
    [HttpPost]
    [ProducesResponseType(typeof(List<PointsChartItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetChartDataAsync([FromBody] AdminListRequest request)
        => ProcessAsync<GetChartAction>(request);
}
