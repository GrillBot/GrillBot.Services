using GrillBot.Core.RabbitMQ.V2.Messages;

namespace RemindService.Models.Events;

public class RemindMessageNotifyPayload : IRabbitMessage
{
    public string Topic => "Remind";
    public string Queue => "RemindMessageNotify";

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
