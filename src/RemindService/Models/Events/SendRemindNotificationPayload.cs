using GrillBot.Core.RabbitMQ;

namespace RemindService.Models.Events;

public class SendRemindNotificationPayload : IPayload
{
    public string QueueName => "remind:send_remind";

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
