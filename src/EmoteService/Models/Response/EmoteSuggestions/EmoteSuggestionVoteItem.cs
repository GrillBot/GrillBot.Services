namespace EmoteService.Models.Response.EmoteSuggestions;

public record EmoteSuggestionVoteItem(
    ulong UserId,
    bool IsApproved,
    DateTime VotedAtUtc
);
