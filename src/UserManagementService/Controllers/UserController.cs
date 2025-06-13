using Microsoft.AspNetCore.Mvc;
using UserManagementService.Actions;
using UserManagementService.Models.Response;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace UserManagementService.Controllers;

[Route("api/user")]
public class UserController(IServiceProvider serviceProvider) : ControllerBase(serviceProvider)
{
    [HttpGet("{userId}")]
    [ProducesResponseType<UserInfo>(StatusCodes.Status200OK)]
    public Task<IActionResult> GetUserInfoAsync(ulong userId)
        => ProcessAsync<GetUserInfoAction>(userId);
}
