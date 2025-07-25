﻿using GrillBot.Core.Models.Pagination;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UnverifyService.Actions.Keepables;
using UnverifyService.Models.Request.Keepables;
using UnverifyService.Models.Response.Keepables;

namespace UnverifyService.Controllers;

public class KeepablesController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> CreateKeepablesAsync([FromBody] List<CreateKeepableRequest> requests)
        => ProcessAsync<CreateKeepablesAction>(requests);

    [HttpPost("list")]
    [ProducesResponseType<PaginatedResponse<KeepableListItem>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetKeepablesListAsync([FromBody] KeepablesListRequest request)
        => ProcessAsync<GetKeepablesListAction>(request);

    [HttpDelete("{group}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> DeleteKeepablesAsync(
        [FromRoute, StringLength(100)] string group,
        [FromQuery, StringLength(100)] string? name
    ) => ProcessAsync<DeleteKeepablesAction>(group, name);
}
