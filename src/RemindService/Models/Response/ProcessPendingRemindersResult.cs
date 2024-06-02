namespace RemindService.Models.Response;

public record ProcessPendingRemindersResult(int RemindersCount, List<string> Messages);
