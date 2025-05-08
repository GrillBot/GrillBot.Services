namespace EmoteService.Models.Response.EmoteSuggestions;

public record EmoteSuggestionItem(
    Guid Id,
    ulong FromUserId,
    string Name,
    DateTime SuggestedAtUtc,
    ulong GuildId,
    ulong? NotificationMessageId,
    bool ApprovedForVote,
    ulong? ApprovalUserId,
    DateTime? ApprovedAtUtc,
    string ReasonToAdd,
    DateTime? VoteStartAt,
    DateTime? VoteEndAt,
    DateTime? VoteKilledAt,
    int? UpVotes,
    int? DownVotes
);
