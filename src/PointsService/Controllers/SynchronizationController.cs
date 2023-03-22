using Microsoft.AspNetCore.Mvc;
using PointsService.Actions;
using PointsService.Models;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace PointsService.Controllers;

public class SynchronizationController : ControllerBase
{
    public SynchronizationController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost]
    public Task<IActionResult> ProcessAsync([FromBody] SynchronizationRequest request)
        => ProcessAsync<SynchronizationAction>(request);
}
