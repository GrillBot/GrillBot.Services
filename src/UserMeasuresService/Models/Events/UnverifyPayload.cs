namespace UserMeasuresService.Models.Events;

public class UnverifyPayload : BasePayload
{
    public override string QueueName => "user_measures:unverify";

    public DateTime EndAt { get; set; }
    public long LogSetId { get; set; }

    public UnverifyPayload()
    {
    }

    public UnverifyPayload(DateTime createdAt, string reason, string guildId, string moderatorId, string targetUserId, DateTime endAt, long logSetId)
        : base(createdAt, reason, guildId, moderatorId, targetUserId)
    {
        EndAt = endAt;
        LogSetId = logSetId;
    }
}
