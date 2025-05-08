using EmoteService.Actions.Guild;
using EmoteService.Models.Request.Guild;
using GrillBot.Core.Validation;
using Microsoft.AspNetCore.Mvc;

namespace EmoteService.Controllers;

public class GuildController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpPut("{guildId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public Task<IActionResult> UpdateGuildAsync([FromRoute, DiscordId] ulong guildId, [FromBody] GuildRequest request)
        => ProcessAsync<UpdateGuildAction>(guildId, request);

    [HttpGet("{guildId}")]
    [ProducesResponseType<GuildData>(StatusCodes.Status200OK)]
    public Task<IActionResult> GetGuildAsync([FromRoute, DiscordId] ulong guildId)
        => ProcessAsync<GetGuildAction>(guildId);
}
