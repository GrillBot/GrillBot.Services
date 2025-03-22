using GrillBot.Core.RabbitMQ.V2.Messages;

namespace UserMeasuresService.Models.Events;

public class UnverifyModifyPayload : IRabbitMessage
{
    public string Topic => "UserMeasures";
    public string Queue => "UnverifyModify";

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
