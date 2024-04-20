namespace UserMeasuresService.Models.Events;

public class TimeoutPayload : BasePayload
{
    public override string QueueName => "user_measures:timeout";

    public long ExternalId { get; set; }
    public DateTime ValidToUtc { get; set; }

    public TimeoutPayload()
    {
    }

    public TimeoutPayload(DateTime createdAtUtc, string reason, string guildId, string moderatorId, string targetUserId, DateTime validToUtc, long externalTimeoutId)
        : base(createdAtUtc, reason, guildId, moderatorId, targetUserId)
    {
        ValidToUtc = validToUtc;
        ExternalId = externalTimeoutId;
    }
}
