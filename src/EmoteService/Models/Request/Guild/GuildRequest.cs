using GrillBot.Core.Validation;

namespace EmoteService.Models.Request.Guild;

public class GuildRequest
{
    [DiscordId]
    public string? SuggestionChannelId { get; set; }

    [DiscordId]
    public string? VoteChannelId { get; set; }

    public TimeSpan VoteTime { get; set; }
}
