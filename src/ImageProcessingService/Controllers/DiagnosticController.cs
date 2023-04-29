using GrillBot.Core.Services.Diagnostics;
using GrillBot.Core.Services.Diagnostics.Models;
using Microsoft.AspNetCore.Mvc;

namespace ImageProcessingService.Controllers;

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
    public async Task<ActionResult<DiagnosticInfo>> GetInfoAsync()
        => Ok(await DiagnosticsProvider.GetInfoAsync());
}
