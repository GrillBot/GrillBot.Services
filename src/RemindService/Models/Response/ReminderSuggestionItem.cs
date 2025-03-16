namespace RemindService.Models.Response;

public record ReminderSuggestionItem(
    int RemindId,
    string FromUserId,
    string ToUserId,
    bool IsIncoming,
    DateTime NotifyAtUtc
);
