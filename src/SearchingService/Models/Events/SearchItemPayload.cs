using GrillBot.Core.RabbitMQ.V2.Messages;

namespace SearchingService.Models.Events;

public class SearchItemPayload : IRabbitMessage
{
    public string Topic => "Searching";
    public string Queue => "CreateSearchItem";

    public string UserId { get; set; } = null!;
    public string GuildId { get; set; } = null!;
    public string ChannelId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public DateTime? ValidToUtc { get; set; }
}
