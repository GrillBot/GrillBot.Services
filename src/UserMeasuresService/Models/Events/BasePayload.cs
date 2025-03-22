using GrillBot.Core.RabbitMQ.V2.Messages;

namespace UserMeasuresService.Models.Events;

public abstract class BasePayload : IRabbitMessage
{
    public string Topic => "UserMeasures";
    public abstract string Queue { get; }

    public DateTime CreatedAtUtc { get; set; }
    public string Reason { get; set; } = null!;
    public string GuildId { get; set; } = null!;
    public string ModeratorId { get; set; } = null!;
    public string TargetUserId { get; set; } = null!;

    protected BasePayload()
    {
    }

    protected BasePayload(DateTime createdAtUtc, string reason, string guildId, string moderatorId, string targetUserId)
    {
        CreatedAtUtc = createdAtUtc;
        Reason = reason;
        GuildId = guildId;
        ModeratorId = moderatorId;
        TargetUserId = targetUserId;
    }
}
