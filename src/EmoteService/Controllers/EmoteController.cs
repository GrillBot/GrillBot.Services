using EmoteService.Actions;
using EmoteService.Actions.SupportedEmotes;
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

    [HttpGet("{guildId}/supported/{emoteId}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public Task<IActionResult> GetIsEmoteSupportedAsync(
        [DiscordId, StringLength(32)] string guildId,
        [EmoteId, StringLength(255)] string emoteId
    ) => ProcessAsync<GetIsEmoteSupportedAction>(guildId, emoteId);

    [HttpGet("{guildId}/supported")]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    public Task<IActionResult> GetSupportedEmotesListAsync([DiscordId, StringLength(32)] string guildId)
        => ProcessAsync<GetSupportedEmotesListAction>(guildId);

    [HttpGet("{guildId}/{emoteId}/info")]
    public Task<IActionResult> GetEmoteInfoAsync(
        [DiscordId, StringLength(32)] string guildId,
        [EmoteId, StringLength(255)] string emoteId
    ) => ProcessAsync<GetEmoteInfoAction>(guildId, emoteId);
}
