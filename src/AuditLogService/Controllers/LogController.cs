using AuditLogService.Actions;
using AuditLogService.Models.Request;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace AuditLogService.Controllers;

public class LogController : ControllerBase
{
    public LogController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateItemsAsync(List<LogRequest> requests)
        => await ProcessAsync<CreateItemsAction>(requests);
}
