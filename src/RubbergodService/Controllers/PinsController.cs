using Microsoft.AspNetCore.Mvc;
using RubbergodService.Actions.Pins;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace RubbergodService.Controllers;

public class PinsController : ControllerBase
{
    public PinsController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet("{guildId}/{channelId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPinsAsync(ulong guildId, ulong channelId, bool markdown = false)
        => await ProcessAsync<GetPinsAction>(guildId, channelId, markdown);
}
