using GrillBot.Core.RabbitMQ;

namespace UserMeasuresService.Models.Events;

public class UnverifyModifyPayload : IPayload
{
    public string QueueName => "user_measures:unverify_modify";

    public long LogSetId { get; set; }
    public DateTime? NewEndUtc { get; set; }

    public UnverifyModifyPayload()
    {
    }

    public UnverifyModifyPayload(long logSetId, DateTime? newEndUtc)
    {
        LogSetId = logSetId;
        NewEndUtc = newEndUtc;
    }
}
