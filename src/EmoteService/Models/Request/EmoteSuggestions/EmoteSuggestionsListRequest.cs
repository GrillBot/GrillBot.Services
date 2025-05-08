using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;
using GrillBot.Core.Validation;

namespace EmoteService.Models.Request.EmoteSuggestions;

public class EmoteSuggestionsListRequest
{
    [DiscordId]
    public ulong? GuildId { get; set; }

    [DiscordId]
    public ulong? FromUserId { get; set; }
    public DateTime? SuggestedFrom { get; set; }
    public DateTime? SuggestedTo { get; set; }
    public string? NameContains { get; set; }
    public bool? ApprovalState { get; set; }

    public PaginatedParams Pagination { get; set; } = new();

    /// <summary>
    /// Available: SuggestedAt, Name
    /// Default: SuggestedAt
    /// </summary>
    public SortParameters Sort { get; set; } = new() { OrderBy = "SuggestedAt" };
}
