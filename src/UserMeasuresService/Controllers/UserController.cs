using Microsoft.AspNetCore.Mvc;
using UserMeasuresService.Actions.User;
using UserMeasuresService.Models.User;

namespace UserMeasuresService.Controllers;

[Route("api/user")]
public class UserController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpGet("{guildId}/{userId}")]
    [ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserInfoAsync(string guildId, string userId)
        => await ProcessAsync<GetUserInfo>(guildId, userId);
}
