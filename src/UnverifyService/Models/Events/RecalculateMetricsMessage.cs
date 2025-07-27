using GrillBot.Core.RabbitMQ.V2.Messages;

namespace UnverifyService.Models.Events;

public class RecalculateMetricsMessage : IRabbitMessage
{
    public string Topic => "Unverify";
    public string Queue => "RecalculateMetrics";
}
