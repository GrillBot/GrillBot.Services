using GrillBot.Core.Models.Pagination;
using Microsoft.AspNetCore.Mvc;
using UnverifyService.Actions;
using UnverifyService.Models.Request;
using UnverifyService.Models.Response;

namespace UnverifyService.Controllers;

public class UnverifyController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpPost("list")]
    [ProducesResponseType<PaginatedResponse<ActiveUnverifyListItemResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetActiveUnverifyListAsync([FromBody] ActiveUnverifyListRequest request)
        => ProcessAsync<GetActiveUnverifyListAction>(request);

    [HttpPost("list/current-user")]
    [ProducesResponseType<PaginatedResponse<ActiveUnverifyListItemResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public Task<IActionResult> GetCurrentUserUnverifyListAsync([FromBody] ActiveUnverifyListRequest request)
        => ProcessAsync<GetCurrentUserUnverifyListAction>(request);
}
