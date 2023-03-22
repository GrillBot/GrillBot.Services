using Microsoft.AspNetCore.Mvc;
using PointsService.Actions;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace PointsService.Controllers;

public class MergeController : ControllerBase
{
    public MergeController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost]
    public Task<IActionResult> MergeTransctionsAsync()
        => ProcessAsync<MergeTransactionsAction>();
}
