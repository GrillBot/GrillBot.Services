using Microsoft.AspNetCore.Mvc;
using UserMeasuresService.Actions.Info;

namespace UserMeasuresService.Controllers;

[Route("api/info")]
public class InfoController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpGet("count/{guildId}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetItemsCountOfGuildAsync(string guildId)
        => await ProcessAsync<GetItemsCountOfGuild>(guildId);
}
