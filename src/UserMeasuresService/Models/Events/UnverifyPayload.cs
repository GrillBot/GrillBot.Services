namespace UserMeasuresService.Models.Events;

public class UnverifyPayload : BasePayload
{
    public override string QueueName => "user_measures:unverify";

    public DateTime EndAtUtc { get; set; }
    public long LogSetId { get; set; }

    public UnverifyPayload()
    {
    }

    public UnverifyPayload(DateTime createdAtUtc, string reason, string guildId, string moderatorId, string targetUserId, DateTime endAtUtc, long logSetId)
        : base(createdAtUtc, reason, guildId, moderatorId, targetUserId)
    {
        EndAtUtc = endAtUtc;
        LogSetId = logSetId;
    }
}
