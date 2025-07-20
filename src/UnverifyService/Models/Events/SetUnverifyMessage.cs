using GrillBot.Core.RabbitMQ.V2.Messages;
using UnverifyService.Models.Request;

namespace UnverifyService.Models.Events;

public class SetUnverifyMessage : UnverifyRequest, IRabbitMessage
{
    public string Topic => "Unverify";
    public string Queue => "SetUnverify";
}
