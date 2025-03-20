using Microsoft.AspNetCore.Mvc;
using PointsService.Actions.Merge;
using PointsService.Models;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace PointsService.Controllers;

public class MergeController(IServiceProvider serviceProvider) : ControllerBase(serviceProvider)
{
    [HttpPost]
    [ProducesResponseType(typeof(MergeResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public Task<IActionResult> MergeTransctionsAsync()
        => ProcessAsync<MergeValidTransactionsAction>();
}
