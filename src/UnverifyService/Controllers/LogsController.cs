using GrillBot.Core.Models.Pagination;
using Microsoft.AspNetCore.Mvc;
using UnverifyService.Actions.Logs;
using UnverifyService.Models.Request.Logs;
using UnverifyService.Models.Response.Logs;
using UnverifyService.Models.Response.Logs.Detail;

namespace UnverifyService.Controllers;

public class LogsController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpPost("list")]
    [ProducesResponseType<PaginatedResponse<UnverifyLogItem>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetUnverifyLogsAsync([FromBody] UnverifyLogListRequest request)
        => ProcessAsync<GetUnverifyLogsAction>(request);

    [HttpGet("{id:guid}")]
    [ProducesResponseType<UnverifyLogDetail>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetUnverifyLogDetailAsync(Guid id)
        => ProcessAsync<GetUnverifyLogDetailAction>(id);
}
