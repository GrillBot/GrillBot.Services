using GrillBot.Core.Services.Diagnostics;
using GrillBot.Core.Services.Diagnostics.Models;
using Microsoft.AspNetCore.Mvc;
using PointsService.Actions;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace PointsService.Controllers;

[ApiController]
[Route("api/diag")]
public class DiagnosticController : ControllerBase
{
    private IDiagnosticsProvider Provider { get; }

    public DiagnosticController(IDiagnosticsProvider provider, IServiceProvider serviceProvider) : base(serviceProvider)
    {
        Provider = provider;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DiagnosticInfo>> GetInfo()
        => Ok(await Provider.GetInfoAsync());

    [HttpGet("status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatusInfoAsync()
        => await ProcessAsync<GetStatusInfoAction>();
}
