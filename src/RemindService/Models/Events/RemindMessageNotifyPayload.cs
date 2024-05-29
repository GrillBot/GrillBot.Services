using GrillBot.Core.RabbitMQ;

namespace RemindService.Models.Events;

public class RemindMessageNotifyPayload : IPayload
{
    public string QueueName => "remind:message_notify";

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
