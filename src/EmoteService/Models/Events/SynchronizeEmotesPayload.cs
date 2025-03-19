using Discord;

namespace EmoteService.Models.Events;

public class SynchronizeEmotesPayload
{
    public string GuildId { get; set; } = null!;
    public List<string> Emotes { get; set; } = [];

    public SynchronizeEmotesPayload()
    {
    }

    public SynchronizeEmotesPayload(string guildId, IEnumerable<string> emotes)
    {
        GuildId = guildId;
        Emotes.AddRange(emotes);
    }

    public SynchronizeEmotesPayload(string guildId, IEnumerable<IEmote> emotes) : this(guildId, emotes.Select(o => o.ToString()!))
    {
    }
}
