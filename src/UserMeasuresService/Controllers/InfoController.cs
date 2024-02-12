using Microsoft.AspNetCore.Mvc;
using UserMeasuresService.Actions.Info;

namespace UserMeasuresService.Controllers;

[Route("api/info")]
public class InfoController : GrillBot.Core.Infrastructure.Actions.ControllerBase
{
    public InfoController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet("count/{guildId}")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetItemsCountOfGuildAsync(string guildId)
        => await ProcessAsync<GetItemsCountOfGuild>(guildId);
}
