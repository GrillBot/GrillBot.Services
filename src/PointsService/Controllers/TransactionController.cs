using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;
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

    [HttpPost("transfer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> TransferPointsAsync([FromBody] TransferPointsRequest request)
        => ProcessAsync<TransferPointsAction>(request);

    [HttpGet("{guildId}/{userId}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public Task<IActionResult> ExistsAnyAsync([DiscordId, StringLength(30)] string guildId, [DiscordId, StringLength(30)] string userId)
        => ProcessAsync<TransactionExistsAction>(guildId, userId);
}
