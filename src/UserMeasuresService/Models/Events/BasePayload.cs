namespace UserMeasuresService.Models.Events;

public abstract class BasePayload
{
    public DateTime CreatedAt { get; set; }
    public string Reason { get; set; } = null!;
    public string GuildId { get; set; } = null!;
    public string ModeratorId { get; set; } = null!;
    public string TargetUserId { get; set; } = null!;

    protected BasePayload()
    {
    }

    protected BasePayload(DateTime createdAt, string reason, string guildId, string moderatorId, string targetUserId)
    {
        CreatedAt = createdAt;
        Reason = reason;
        GuildId = guildId;
        ModeratorId = moderatorId;
        TargetUserId = targetUserId;
    }
}
