using AuditLogService.Actions.Internal;
using AuditLogService.Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AuditLogService.Controllers;

public class InternalController : GrillBot.Core.Infrastructure.Actions.ControllerBase
{
    public InternalController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet("recalculation/{type}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<IActionResult> TriggerRecalculationAsync(LogType type)
        => ProcessAsync<TriggerRecalculationAction>(type);
}
