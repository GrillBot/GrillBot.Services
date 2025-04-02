using GrillBot.Core.Models.Pagination;
using InviteService.Actions;
using InviteService.Models.Request;
using InviteService.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace InviteService.Controllers;

public class InviteController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpPost("cached-invites/list")]
    [ProducesResponseType<PaginatedResponse<Invite>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetCachedInvitesAsync([FromBody] InviteListRequest request)
        => ProcessAsync<GetCachedInvitesAction>(request);

    [HttpPost("used-invites/list")]
    [ProducesResponseType<PaginatedResponse<Invite>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetUsedInvitesAsync([FromBody] InviteListRequest request)
        => ProcessAsync<GetUsedInvitesAction>(request);

    [HttpPost("invite-uses/list")]
    [ProducesResponseType<PaginatedResponse<InviteUse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetInviteUsesAsync([FromBody] InviteUseListRequest request)
        => ProcessAsync<GetInviteUsesAction>(request);

    [HttpPost("invite-user-uses/list")]
    [ProducesResponseType<PaginatedResponse<UserInviteUse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserInviteUsesAsync([FromBody] UserInviteUseListRequest request)
        => await ProcessAsync<GetUserInviteUsesAction>(request);
}
