using Microsoft.AspNetCore.Mvc;
using RubbergodService.Actions;
using RubbergodService.Core.Models;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace RubbergodService.Controllers;

public class DirectApiController : ControllerBase
{
    public DirectApiController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost("{service}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> SendAsync(string service, DirectApiCommand command)
        => ProcessAsync<SendDirectApiAction>(service, command);
}
