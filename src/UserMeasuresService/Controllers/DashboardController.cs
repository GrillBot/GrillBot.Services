using Microsoft.AspNetCore.Mvc;
using UserMeasuresService.Actions.Dashboard;
using UserMeasuresService.Models.Dashboard;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace UserMeasuresService.Controllers;

[Route("api/dashboard")]
public class DashboardController(IServiceProvider serviceProvider) : ControllerBase(serviceProvider)
{
    [HttpGet]
    [ProducesResponseType(typeof(List<DashboardRow>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDashboardDataAsync()
        => await ProcessAsync<GetDashboardData>();
}
