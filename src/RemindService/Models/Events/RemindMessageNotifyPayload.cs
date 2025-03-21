namespace RemindService.Models.Events;

public class RemindMessageNotifyPayload
{
    public long RemindId { get; set; }
    public string NotificationMessageId { get; set; } = null!;

    public RemindMessageNotifyPayload()
    {
    }

    public RemindMessageNotifyPayload(long remindId, string notificationMessageId)
    {
        RemindId = remindId;
        NotificationMessageId = notificationMessageId;
    }
}
