using EmoteService.Actions;
using EmoteService.Actions.SupportedEmotes;
using EmoteService.Models.Response;
using GrillBot.Core.Validation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace EmoteService.Controllers;

public class EmoteController(IServiceProvider serviceProvider) : ControllerBase(serviceProvider)
{
    [HttpGet("supported")]
    [ProducesResponseType(typeof(List<EmoteDefinition>), StatusCodes.Status200OK)]
    public Task<IActionResult> GetSupportedEmotesListAsync(string? guildId = null)
        => ProcessAsync<GetSupportedEmotesListAction>(guildId);

    [HttpGet("{guildId}/{emoteId}/info")]
    [ProducesResponseType(typeof(EmoteInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetEmoteInfoAsync(
        [DiscordId, StringLength(32)] string guildId,
        [EmoteId, StringLength(255)] string emoteId
    ) => ProcessAsync<GetEmoteInfoAction>(guildId, emoteId);

    [HttpDelete("supported/{guildId}/{emoteId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> DeleteSupportedEmoteAsync(
        [StringLength(32), DiscordId] string guildId,
        [StringLength(255), EmoteId] string emoteId
    ) => ProcessAsync<DeleteSupportedEmoteAction>(guildId, emoteId);
}
