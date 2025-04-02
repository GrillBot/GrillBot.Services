using GrillBot.Core.Models.Pagination;
using InviteService.Actions;
using InviteService.Models.Request;
using InviteService.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace InviteService.Controllers;

public class InviteController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpPost("cached/list")]
    [ProducesResponseType<PaginatedResponse<Invite>>(StatusCodes.Status200OK)]
    public Task<IActionResult> GetCachedInvitesAsync([FromBody] InviteListRequest request)
        => ProcessAsync<GetCachedInvitesAction>(request);

    [HttpPost("used/list")]
    [ProducesResponseType<PaginatedResponse<Invite>>(StatusCodes.Status200OK)]
    public Task<IActionResult> GetUsedInvitesAsync([FromBody] InviteListRequest request)
        => ProcessAsync<GetUsedInvitesAction>(request);
}
