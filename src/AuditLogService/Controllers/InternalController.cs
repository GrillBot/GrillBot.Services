using AuditLogService.Actions.Internal;
using AuditLogService.Core.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AuditLogService.Controllers;

public class InternalController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpGet("recalculation/{type}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<IActionResult> TriggerRecalculationAsync(LogType type)
        => ProcessAsync<TriggerRecalculationAction>(type);
}
