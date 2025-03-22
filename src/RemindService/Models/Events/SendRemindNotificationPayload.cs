using GrillBot.Core.RabbitMQ.V2.Messages;

namespace RemindService.Models.Events;

public class SendRemindNotificationPayload : IRabbitMessage
{
    public string Topic => "Remind";
    public string Queue => "SendRemindNotification";

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
