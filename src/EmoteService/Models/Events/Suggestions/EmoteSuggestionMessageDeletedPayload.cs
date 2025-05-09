using Discord;
using GrillBot.Core.RabbitMQ.V2.Messages;

namespace EmoteService.Models.Events.Suggestions;

public class EmoteSuggestionMessageDeletedPayload : IRabbitMessage
{
    public string Topic => "Emote";
    public string Queue => "EmoteSuggestionMessageDeleted";

    public ulong GuildId { get; set; }
    public ulong MessageId { get; set; }

    public EmoteSuggestionMessageDeletedPayload()
    {
    }

    public EmoteSuggestionMessageDeletedPayload(ulong guildId, ulong messageId)
    {
        GuildId = guildId;
        MessageId = messageId;
    }

    public static EmoteSuggestionMessageDeletedPayload Create(IGuild guild, IMessage message)
        => new(guild.Id, message.Id);
}
