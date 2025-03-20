using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;
using Microsoft.AspNetCore.Mvc;
using PointsService.Actions;
using PointsService.Models;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace PointsService.Controllers;

public class TransactionController(IServiceProvider serviceProvider) : ControllerBase(serviceProvider)
{
    [HttpPost("transfer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> TransferPointsAsync([FromBody] TransferPointsRequest request)
        => ProcessAsync<ProcessTransferPointsAction>(request);

    [HttpPost("increment")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> IncrementPointsAsync([FromBody] IncrementPointsRequest request)
        => ProcessAsync<IncrementPointsAction>(request);

    [HttpGet("{guildId}/{userId}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> ExistsAnyAsync([DiscordId, StringLength(30)] string guildId, [DiscordId, StringLength(30)] string userId)
        => ProcessAsync<CheckTransactionExistsAction>(guildId, userId);

    [HttpGet("{guildId}/count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetTransactionsCountForGuildActionAsync([DiscordId, StringLength(30)] string guildId)
        => ProcessAsync<GetTransactionsCountForGuildAction>(guildId);
}
