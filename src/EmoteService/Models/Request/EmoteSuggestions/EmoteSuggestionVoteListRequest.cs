using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;
using GrillBot.Core.Validation;

namespace EmoteService.Models.Request.EmoteSuggestions;

public class EmoteSuggestionVoteListRequest
{
    [DiscordId]
    public string? UserId { get; set; }

    public PaginatedParams Pagination { get; set; } = new();
    public SortParameters Sort { get; set; } = new();
}
