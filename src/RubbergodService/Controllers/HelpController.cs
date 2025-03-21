using Microsoft.AspNetCore.Mvc;
using RubbergodService.Models;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace RubbergodService.Controllers;

public class HelpController(IServiceProvider serviceProvider) : ControllerBase(serviceProvider)
{
    [HttpGet("slashCommands")]
    [ProducesResponseType(typeof(Dictionary<string, Cog>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSlashCommandsAsync()
        => await ProcessAsync<Actions.Help.GetSlashCommandsAction>();
}
