using GrillBot.Core.Models.Pagination;
using Microsoft.AspNetCore.Mvc;
using RubbergodService.Actions.Karma;
using RubbergodService.Core.Entity;
using RubbergodService.Core.Models;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace RubbergodService.Controllers;

public class KarmaController : ControllerBase
{
    public KarmaController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> StoreKarmaAsync(List<Karma> items)
        => ProcessAsync<StoreKarmaAction>(items);

    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<UserKarma>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetPageAsync([FromQuery] PaginatedParams parameters)
        => ProcessAsync<GetKarmaPageAction>(parameters);
}
