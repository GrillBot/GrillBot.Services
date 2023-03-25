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

    [HttpDelete("{guildId}/{messageId}")]
    public Task<IActionResult> DeleteTransactionAsync([DiscordId, StringLength(30)] string guildId, [DiscordId, StringLength(30)] string messageId)
        => ProcessAsync<DeleteTransactionsAction>(guildId, messageId);

    [HttpDelete("{guildId}/{messageId}/{reactionId}")]
    public Task<IActionResult> DeleteTransactionAsync([DiscordId, StringLength(30)] string guildId, [DiscordId, StringLength(30)] string messageId, [StringLength(100)] string reactionId)
        => ProcessAsync<DeleteTransactionsAction>(guildId, messageId, reactionId);

    [HttpPost("transfer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> TransferPointsAsync([FromBody] TransferPointsRequest request)
        => ProcessAsync<TransferPointsAction>(request);
}
