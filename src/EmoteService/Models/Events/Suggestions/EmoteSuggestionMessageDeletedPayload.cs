using GrillBot.Core.RabbitMQ.V2.Messages;

namespace EmoteService.Models.Events.Suggestions;

public class EmoteSuggestionMessageDeletedPayload : IRabbitMessage
{
    public string Topic => "Emote";
    public string Queue => "EmoteSuggestionMessageDeleted";

    public ulong GuildId { get; set; }
    public ulong MessageId { get; set; }
}
