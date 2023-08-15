using Microsoft.AspNetCore.Mvc;
using RubbergodService.Core.Models;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace RubbergodService.Controllers;

public class HelpController : ControllerBase
{
    public HelpController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet("slashCommands")]
    [ProducesResponseType(typeof(Dictionary<string, Cog>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSlashCommandsAsync()
        => await ProcessAsync<Actions.Help.GetSlashCommandsAction>();
}
