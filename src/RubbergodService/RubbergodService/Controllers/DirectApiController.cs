using Microsoft.AspNetCore.Mvc;
using RubbergodService.Core.Helpers;
using RubbergodService.Core.Models;
using RubbergodService.DirectApi;

namespace RubbergodService.Controllers;

[ApiController]
[Route("api/directApi")]
public class DirectApiController : Controller
{
    private DirectApiManager DirectApiManager { get; }

    public DirectApiController(DirectApiManager directApiManager)
    {
        DirectApiManager = directApiManager;
    }

    [HttpPost("{service}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> SendAsync(string service, DirectApiCommand command)
    {
        using var response = await DirectApiManager.SendAsync(service, command);
        var json = await JsonHelper.SerializeJsonDocumentAsync(response);

        return Ok(json);
    }
}
