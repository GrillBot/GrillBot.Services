using EmoteService.Actions;
using EmoteService.Actions.SupportedEmotes;
using EmoteService.Models.Response;
using GrillBot.Core.Validation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace EmoteService.Controllers;

public class EmoteController : ControllerBase
{
    public EmoteController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet("supported/{guildId}/{emoteId}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public Task<IActionResult> GetIsEmoteSupportedAsync(
        [DiscordId, StringLength(32)] string guildId,
        [EmoteId, StringLength(255)] string emoteId
    ) => ProcessAsync<GetIsEmoteSupportedAction>(guildId, emoteId);

    [HttpGet("supported")]
    [ProducesResponseType(typeof(List<EmoteDefinition>), StatusCodes.Status200OK)]
    public Task<IActionResult> GetSupportedEmotesListAsync()
        => ProcessAsync<GetSupportedEmotesListAction>();

    [HttpGet("{guildId}/{emoteId}/info")]
    [ProducesResponseType(typeof(EmoteInfo), StatusCodes.Status200OK)]
    public Task<IActionResult> GetEmoteInfoAsync(
        [DiscordId, StringLength(32)] string guildId,
        [EmoteId, StringLength(255)] string emoteId
    ) => ProcessAsync<GetEmoteInfoAction>(guildId, emoteId);
}
