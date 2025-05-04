using GrillBot.Core.RabbitMQ.V2.Messages;

namespace EmoteService.Models.Events.Suggestions;

public class EmoteSuggestionMessageCreatedPayload : IRabbitMessage
{
    public string Topic => "Emote";
    public string Queue => "EmoteSuggestionMessageCreated";

    public Guid SuggestionId { get; set; }
    public ulong MessageId { get; set; }
}
