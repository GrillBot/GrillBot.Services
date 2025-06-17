using GrillBot.Core.RabbitMQ.V2.Messages;
using MessageService.Models.Events.Channels;

namespace MessageService.Models.Events;

public class SynchronizationPayload : IRabbitMessage
{
    public string Topic => "Message";
    public string Queue => "Synchronization";

    public List<ChannelSynchronizationItem> Channels { get; set; } = [];

    public SynchronizationPayload()
    {
    }

    public SynchronizationPayload(List<ChannelSynchronizationItem> channels)
    {
        Channels = channels;
    }
}
