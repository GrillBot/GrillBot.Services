using AuditLogService.Actions.Detail;
using AuditLogService.Actions.Search;
using AuditLogService.Models.Request.Search;
using AuditLogService.Models.Response.Search;
using GrillBot.Core.Models.Pagination;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace AuditLogService.Controllers;

public class LogItemController : ControllerBase
{
    public LogItemController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(PaginatedResponse<LogListItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SearchItemsAsync(SearchRequest request)
        => await ProcessAsync<SearchItemsAction>(request);

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DetailAsync(Guid id)
        => await ProcessAsync<ReadDetailAction>(id);
}
