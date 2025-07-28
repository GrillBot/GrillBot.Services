using GrillBot.Core.Validation;
using Microsoft.AspNetCore.Mvc;
using UnverifyService.Actions.Guilds;
using UnverifyService.Models.Response.Guilds;

namespace UnverifyService.Controllers;

public class GuildController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpGet("{guildId}")]
    [ProducesResponseType<GuildInfo>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> GetGuildInfoAsync([FromRoute, DiscordId] ulong guildId)
        => ProcessAsync<GetGuildInfoAction>(guildId);
}
