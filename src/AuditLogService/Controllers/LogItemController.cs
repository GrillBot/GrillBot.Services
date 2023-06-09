using AuditLogService.Actions;
using AuditLogService.Models.Request;
using AuditLogService.Models.Request.CreateItems;
using AuditLogService.Models.Response;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace AuditLogService.Controllers;

public class LogItemController : ControllerBase
{
    public LogItemController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateItemsAsync(List<LogRequest> requests)
        => await ProcessAsync<CreateItemsAction>(requests);

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(DeleteItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DeleteItemResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteItemAsync(Guid id)
        => await ProcessAsync<DeleteItemAction>(id);
}
