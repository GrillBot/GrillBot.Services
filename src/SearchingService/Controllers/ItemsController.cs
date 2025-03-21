﻿using GrillBot.Core.Models.Pagination;
using GrillBot.Core.Validation;
using Microsoft.AspNetCore.Mvc;
using SearchingService.Actions;
using SearchingService.Models.Request;
using SearchingService.Models.Response;
using System.ComponentModel.DataAnnotations;

namespace SearchingService.Controllers;

public class ItemsController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpPost("list")]
    [ProducesResponseType(typeof(PaginatedResponse<SearchListItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetSearchingListAsync([FromBody] SearchingListRequest request)
        => ProcessAsync<GetSearchingListAction>(request);

    [HttpGet("suggestions/{guildId}/{channelId}")]
    [ProducesResponseType(typeof(List<SearchSuggestion>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetSuggestionsAsync(
        [StringLength(30), DiscordId] string guildId,
        [StringLength(30), DiscordId] string channelId
    ) => ProcessAsync<GetSearchingSuggestionsAction>(guildId, channelId);

    [HttpDelete("remove/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> RemoveSearchingAsync(long id)
        => ProcessAsync<RemoveSearchingAction>(id);
}
