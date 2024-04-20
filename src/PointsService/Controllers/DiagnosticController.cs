using Microsoft.AspNetCore.Mvc;
using PointsService.Actions;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace PointsService.Controllers;

[ApiController]
[Route("api/diag")]
public class DiagnosticController : ControllerBase
{
    public DiagnosticController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet("status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatusInfoAsync()
        => await ProcessAsync<GetStatusInfoAction>();
}
