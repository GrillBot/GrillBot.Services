using GrillBot.Core.Services.Diagnostics;
using GrillBot.Core.Services.Diagnostics.Models;
using Microsoft.AspNetCore.Mvc;

namespace RubbergodService.Controllers;

[ApiController]
[Route("api/diag")]
public class DiagnosticController : Controller
{
    private IDiagnosticsProvider DiagnosticsProvider { get; }

    public DiagnosticController(IDiagnosticsProvider diagnosticsProvider)
    {
        DiagnosticsProvider = diagnosticsProvider;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DiagnosticInfo>> GetInfoAsync()
        => Ok(await DiagnosticsProvider.GetInfoAsync());
}
