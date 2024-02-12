using GrillBot.Core.Services.Diagnostics;
using GrillBot.Core.Services.Diagnostics.Models;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace UserMeasuresService.Controllers;

[Route("api/diag")]
public class DiagnosticController : ControllerBase
{
    private IDiagnosticsProvider DiagnosticsProvider { get; }

    public DiagnosticController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
        DiagnosticsProvider = serviceProvider.GetRequiredService<IDiagnosticsProvider>();
    }

    [HttpGet]
    [ProducesResponseType(typeof(DiagnosticInfo), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInfoAsync()
        => Ok(await DiagnosticsProvider.GetInfoAsync());
}
