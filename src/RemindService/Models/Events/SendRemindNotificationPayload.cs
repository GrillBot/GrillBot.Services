using GrillBot.Core.RabbitMQ;

namespace RemindService.Models.Events;

public class SendRemindNotificationPayload : IPayload
{
    public string QueueName => "remind:send_remind";

    public long RemindId { get; set; }
    public bool IsEarly { get; set; }

    public SendRemindNotificationPayload()
    {
    }

    public SendRemindNotificationPayload(long remindId, bool isEarly)
    {
        RemindId = remindId;
        IsEarly = isEarly;
    }
}
