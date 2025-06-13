using Microsoft.AspNetCore.Mvc;
using UserManagementService.Actions;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace UserManagementService.Controllers;

[Route("api/guild/{guildId}/users")]
public class GuildUserController(IServiceProvider serviceProvider) : ControllerBase(serviceProvider)
{
    [HttpGet("with-nickname")]
    [ProducesResponseType<Dictionary<string, string>>(StatusCodes.Status200OK)]
    public Task<IActionResult> GetGuildUsersWithNicknameAsync([FromRoute] ulong guildId)
        => ProcessAsync<GetGuildUsersWithNicknameAction>(guildId);
}
