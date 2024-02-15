using Microsoft.AspNetCore.Mvc;
using PointsService.Actions;
using PointsService.Models;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace PointsService.Controllers;

public class ChartController : ControllerBase
{
    public ChartController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost]
    [ProducesResponseType(typeof(List<PointsChartItem>), StatusCodes.Status200OK)]
    public Task<IActionResult> GetChartDataAsync([FromBody] AdminListRequest request)
        => ProcessAsync<GetChartAction>(request);
}
