using GrillBot.Core.Services.Diagnostics;
using GrillBot.Core.Services.Diagnostics.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GrillBot.Services.Common.Infrastructure.Api.Controllers;

[ApiController]
[Route("api/diag")]
public class ServiceDiagnosticsController : Core.Infrastructure.Actions.ControllerBase
{
    private readonly IDiagnosticsProvider _diagnosticsProvider;

    public ServiceDiagnosticsController(IDiagnosticsProvider diagnosticsProvider, IServiceProvider serviceProvider) : base(serviceProvider)
    {
        _diagnosticsProvider = diagnosticsProvider;
    }

    [HttpGet]
    [ProducesResponseType(typeof(DiagnosticInfo), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDiagnosticsAsync()
        => Ok(await _diagnosticsProvider.GetInfoAsync());
}
