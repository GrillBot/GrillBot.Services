namespace EmoteService.Models.Response.EmoteSuggestions;

public record EmoteSuggestionItem(
    Guid Id,
    string FromUserId,
    string Name,
    DateTime SuggestedAtUtc,
    string GuildId,
    string? NotificationMessageId,
    bool ApprovedForVote,
    string? ApprovalUserId,
    DateTime? ApprovedAtUtc,
    string ReasonToAdd,
    DateTime? VoteStartAt,
    DateTime? VoteEndAt,
    DateTime? VoteKilledAt,
    int? UpVotes,
    int? DownVotes
);
