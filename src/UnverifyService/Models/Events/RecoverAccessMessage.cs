using GrillBot.Core.RabbitMQ.V2.Messages;

namespace UnverifyService.Models.Events;

public class RecoverAccessMessage : IRabbitMessage
{
    public string Topic => "Unverify";
    public string Queue => "RecoverAccess";

    public long? LogNumber { get; set; }
    public Guid? LogId { get; set; }
}
