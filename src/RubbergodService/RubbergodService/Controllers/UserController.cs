using Microsoft.AspNetCore.Mvc;
using RubbergodService.Actions;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace RubbergodService.Controllers;

public class UserController : ControllerBase
{
    public UserController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPatch("{memberId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<IActionResult> RefreshUserAsync(string memberId)
        => ProcessAsync<RefreshUserAction>(memberId);
}
