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
}
