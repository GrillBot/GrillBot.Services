using GrillBot.Core.RabbitMQ.V2.Messages;

namespace EmoteService.Models.Events.Suggestions;

public class EmoteSuggestionVoteMessageCreatedPayload : IRabbitMessage
{
    public string Topic => "Emote";
    public string Queue => "EmoteSuggestionVoteMessageCreated";

    public Guid SuggestionId { get; set; }
    public ulong MessageId { get; set; }

    public EmoteSuggestionVoteMessageCreatedPayload(Guid suggestionId, ulong messageId)
    {
        SuggestionId = suggestionId;
        MessageId = messageId;
    }

    public EmoteSuggestionVoteMessageCreatedPayload()
    {
    }
}
