namespace RemindService.Models.Response;

public record ReminderSuggestionItem(
    long RemindId,
    string FromUserId,
    string ToUserId,
    bool IsIncoming,
    DateTime NotifyAtUtc
);
