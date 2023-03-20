using Microsoft.AspNetCore.Mvc;
using PointsService.Actions;
using PointsService.Models;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace PointsService.Controllers;

public class TransactionController : ControllerBase
{
    public TransactionController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpPost]
    public Task<IActionResult> CreateTransactionAsync([FromBody] TransactionRequest request)
        => ProcessAsync<CreateTransactionAction>(request);
}
