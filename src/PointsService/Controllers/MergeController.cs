using Microsoft.AspNetCore.Mvc;
using PointsService.Actions;
using PointsService.Models;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace PointsService.Controllers;

public class MergeController : ControllerBase
{
    public MergeController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost]
    [ProducesResponseType(typeof(MergeResult), StatusCodes.Status200OK)]
    public Task<IActionResult> MergeTransctionsAsync()
        => ProcessAsync<MergeTransactionsAction>();
}
