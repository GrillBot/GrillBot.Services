using GrillBot.Core.Validation;

namespace EmoteService.Models.Request.Guild;

public class GuildRequest
{
    [DiscordId]
    public ulong? SuggestionChannelId { get; set; }

    [DiscordId]
    public ulong? VoteChannelId { get; set; }

    public TimeSpan VoteTime { get; set; }
}
