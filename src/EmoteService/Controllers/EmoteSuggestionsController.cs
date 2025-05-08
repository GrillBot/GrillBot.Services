using EmoteService.Actions.EmoteSuggestions;
using EmoteService.Models.Request.EmoteSuggestions;
using EmoteService.Models.Response.EmoteSuggestions;
using GrillBot.Core.Models.Pagination;
using GrillBot.Services.Common.Infrastructure.Api.OpenApi.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace EmoteService.Controllers;

public class EmoteSuggestionsController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpPost("list")]
    [ProducesResponseType<PaginatedResponse<EmoteSuggestionItem>>(StatusCodes.Status200OK)]
    public Task<IActionResult> GetEmoteSuggestionsListAsync([FromBody] EmoteSuggestionsListRequest request)
        => ProcessAsync<GetEmoteSuggestionListAction>(request);

    [HttpPut("approve/{suggestionId:guid}")]
    [SwaggerRequireAuthorization]
    public Task<IActionResult> SetSuggestionApprovalAsync([FromRoute] Guid suggestionId, [FromQuery] bool isApproved)
        => ProcessAsync<SetSuggestionApprovalAction>(suggestionId, isApproved);

    [HttpPost("{suggestionId:guid}/votes")]
    [ProducesResponseType<PaginatedResponse<EmoteSuggestionVoteItem>>(StatusCodes.Status200OK)]
    public Task<IActionResult> GetEmoteSuggestionVotesAsync([FromRoute] Guid suggestionId, [FromBody] EmoteSuggestionVoteListRequest request)
        => ProcessAsync<GetEmoteSuggestionVotesAction>(suggestionId, request);

    [HttpPost("vote/{guildId}")]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    public Task<IActionResult> StartSuggestionsVotingAsync([FromRoute] ulong guildId)
        => ProcessAsync<StartSuggestionsVotingAction>(guildId);

    [HttpPost("vote/finish")]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    public Task<IActionResult> FinishSuggestionVotesAsync()
        => ProcessAsync<FinishSuggestionVotesAction>();
}
