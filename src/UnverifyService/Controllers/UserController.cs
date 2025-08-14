using GrillBot.Core.Validation;
using GrillBot.Services.Common.Infrastructure.Api.OpenApi.Attributes;
using Microsoft.AspNetCore.Mvc;
using UnverifyService.Actions.Users;
using UnverifyService.Models.Request.Users;
using UnverifyService.Models.Response.Users;

namespace UnverifyService.Controllers;

public class UserController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpPut("{userId}")]
    [SwaggerRequireAuthorization]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> ModifyUserAsync(
        [FromRoute, DiscordId] ulong userId,
        [FromBody] ModifyUserRequest request
    ) => ProcessAsync<ModifyUserAction>(userId, request);

    [HttpGet("{userId}")]
    [ProducesResponseType<UserInfo>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetUserInfoAsync(
        [FromRoute, DiscordId] ulong userId
    ) => ProcessAsync<GetUserInfoAction>(userId);
}
