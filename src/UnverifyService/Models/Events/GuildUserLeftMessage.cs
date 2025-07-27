using GrillBot.Core.RabbitMQ.V2.Messages;

namespace UnverifyService.Models.Events;

public class GuildUserLeftMessage : IRabbitMessage
{
    public string Topic => "Unverify";
    public string Queue => "GuildUserLeft";

    public ulong GuildId { get; set; }
    public ulong UserId { get; set; }
}
