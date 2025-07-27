using GrillBot.Core.Validation;
using Microsoft.AspNetCore.Mvc;
using UnverifyService.Actions.Users;
using UnverifyService.Models.Request.Users;

namespace UnverifyService.Controllers;

public class UserController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpPut("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> ModifyUserAsync(
        [FromRoute, DiscordId] ulong userId,
        [FromBody] ModifyUserRequest request
    ) => ProcessAsync<ModifyUserAction>(userId, request);
}
