namespace RemindService.Models.Response;

public record ReminderSuggestionItem(
    string FromUserId,
    string ToUserId,
    bool IsIncoming,
    DateTime NotifyAtUtc
);
