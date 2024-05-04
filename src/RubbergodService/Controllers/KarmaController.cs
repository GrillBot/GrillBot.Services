using GrillBot.Core.Models.Pagination;
using Microsoft.AspNetCore.Mvc;
using RubbergodService.Actions.Karma;
using RubbergodService.Models;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace RubbergodService.Controllers;

public class KarmaController : ControllerBase
{
    public KarmaController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<UserKarma>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetPageAsync([FromQuery] PaginatedParams parameters)
        => ProcessAsync<GetKarmaPageAction>(parameters);
}
