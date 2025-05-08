namespace EmoteService.Models.Request.Guild;

public record GuildData(
    ulong? SuggestionChannelId,
    ulong? VoteChannelId,
    TimeSpan VoteTime
);
