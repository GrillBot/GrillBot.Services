using GrillBot.Core.RabbitMQ;

namespace EmoteService.Models.Events;

public class EmoteEventPayload : IPayload
{
    public string QueueName => "emote:emote_event";

    public string GuildId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string EmoteId { get; set; } = null!;
    public bool IsIncrement { get; set; }
    public DateTime EventCreatedAt { get; set; }

    public EmoteEventPayload()
    {
    }

    public EmoteEventPayload(string guildId, string userId, string emoteId, DateTime eventCreatedAt, bool isIncrement)
    {
        GuildId = guildId;
        UserId = userId;
        EmoteId = emoteId;
        IsIncrement = isIncrement;
        EventCreatedAt = eventCreatedAt;
    }

    public static EmoteEventPayload Increment(string guildId, string userId, string emoteId)
        => new(guildId, userId, emoteId, DateTime.UtcNow, true);

    public static EmoteEventPayload Decrement(string guildId, string userId, string emoteId)
        => new(guildId, userId, emoteId, DateTime.UtcNow, false);
}
