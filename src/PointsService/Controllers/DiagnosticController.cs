using GrillBot.Core.Services.Diagnostics;
using GrillBot.Core.Services.Diagnostics.Models;
using Microsoft.AspNetCore.Mvc;

namespace PointsService.Controllers;

[ApiController]
[Route("api/diag")]
public class DiagnosticController : ControllerBase
{
    private IDiagnosticsProvider Provider { get; }

    public DiagnosticController(IDiagnosticsProvider provider)
    {
        Provider = provider;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DiagnosticInfo>> GetInfo()
        => Ok(await Provider.GetInfoAsync());
}
