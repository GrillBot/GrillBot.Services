using AuditLogService.Actions.Info;
using AuditLogService.Models.Response.Info;
using GrillBot.Core.Services.Diagnostics;
using GrillBot.Core.Services.Diagnostics.Models;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace AuditLogService.Controllers;

[ApiController]
[Route("api/diag")]
public class DiagnosticController : ControllerBase
{
    private IDiagnosticsProvider DiagnosticsProvider { get; }

    public DiagnosticController(IDiagnosticsProvider diagnosticsProvider, IServiceProvider serviceProvider) : base(serviceProvider)
    {
        DiagnosticsProvider = diagnosticsProvider;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<DiagnosticInfo>> GetInfoAsync()
        => Ok(await DiagnosticsProvider.GetInfoAsync());

    [HttpGet("status")]
    [ProducesResponseType(typeof(StatusInfo), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatusInfoAsync()
        => await ProcessAsync<GetStatusInfoAction>();
}
