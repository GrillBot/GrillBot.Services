using EmoteService.Actions.EmoteSuggestions;
using EmoteService.Models.Request.EmoteSuggestions;
using EmoteService.Models.Response.EmoteSuggestions;
using GrillBot.Core.Models.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace EmoteService.Controllers;

public class EmoteSuggestionsController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpPost("list")]
    [ProducesResponseType<PaginatedResponse<EmoteSuggestionItem>>(StatusCodes.Status200OK)]
    public Task<IActionResult> GetEmoteSuggestionsListAsync([FromBody] EmoteSuggestionsListRequest request)
        => ProcessAsync<GetEmoteSuggestionListAction>(request);
}
