namespace RemindService.Models.Events;

public class SendRemindNotificationPayload
{
    public int RemindId { get; set; }
    public bool IsEarly { get; set; }

    public SendRemindNotificationPayload()
    {
    }

    public SendRemindNotificationPayload(int remindId, bool isEarly)
    {
        RemindId = remindId;
        IsEarly = isEarly;
    }
}
