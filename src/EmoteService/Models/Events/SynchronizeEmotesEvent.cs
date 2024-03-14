using Discord;
using GrillBot.Core.RabbitMQ;

namespace EmoteService.Models.Events;

public class SynchronizeEmotesEvent : IPayload
{
    public string QueueName => "emote:synchronize";

    public string GuildId { get; set; } = null!;
    public List<string> Emotes { get; set; } = new();

    public SynchronizeEmotesEvent()
    {
    }

    public SynchronizeEmotesEvent(string guildId, IEnumerable<string> emotes)
    {
        GuildId = guildId;
        Emotes.AddRange(emotes);
    }

    public SynchronizeEmotesEvent(string guildId, IEnumerable<IEmote> emotes) : this(guildId, emotes.Select(o => o.ToString()!))
    {
    }
}
