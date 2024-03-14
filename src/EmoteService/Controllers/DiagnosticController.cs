using GrillBot.Core.Services.Diagnostics;
using GrillBot.Core.Services.Diagnostics.Models;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace EmoteService.Controllers;

[ApiController]
public class DiagnosticController : ControllerBase
{
    private readonly IDiagnosticsProvider _diagnosticsProvider;

    public DiagnosticController(IServiceProvider serviceProvider, IDiagnosticsProvider diagnosticsProvider) : base(serviceProvider)
    {
        _diagnosticsProvider = diagnosticsProvider;
    }

    [HttpGet]
    [ProducesResponseType(typeof(DiagnosticInfo), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInfoAsync()
        => Ok(await _diagnosticsProvider.GetInfoAsync());
}
