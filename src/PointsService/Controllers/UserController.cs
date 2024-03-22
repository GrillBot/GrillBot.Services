using GrillBot.Core.Models.Pagination;
using Microsoft.AspNetCore.Mvc;
using PointsService.Actions.Users;
using PointsService.Models.Users;

namespace PointsService.Controllers;

public class UserController : GrillBot.Core.Infrastructure.Actions.ControllerBase
{
    public UserController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost("list")]
    [ProducesResponseType(typeof(PaginatedResponse<UserListItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserListAsync([FromBody] UserListRequest request)
        => await ProcessAsync<GetUserListAction>(request);

    [HttpGet("{guildId}/{userId}/info")]
    [ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK)]
    public Task<IActionResult> GetUserInfoAsync(string guildId, string userId)
        => ProcessAsync<GetUserInfoAction>(guildId, userId);
}
