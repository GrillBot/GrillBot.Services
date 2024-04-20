using AuditLogService.Actions.Info;
using AuditLogService.Models.Response.Info;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace AuditLogService.Controllers;

[ApiController]
[Route("api/diag")]
public class DiagnosticController : ControllerBase
{
    public DiagnosticController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet("status")]
    [ProducesResponseType(typeof(StatusInfo), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetStatusInfoAsync()
        => await ProcessAsync<GetStatusInfoAction>();
}
