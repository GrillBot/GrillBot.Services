using Microsoft.AspNetCore.Mvc;
using UnverifyService.Actions.Keepables;
using UnverifyService.Models.Request;

namespace UnverifyService.Controllers;

public class KeepablesController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public Task<IActionResult> CreateKeepablesAsync([FromBody] List<CreateKeepableRequest> requests)
        => ProcessAsync<CreateKeepablesAction>(requests);
}
