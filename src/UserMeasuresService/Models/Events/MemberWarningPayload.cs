namespace UserMeasuresService.Models.Events;

public class MemberWarningPayload : BasePayload
{
    public override string QueueName => "user_measures:member_warning";

    public MemberWarningPayload()
    {
    }

    public MemberWarningPayload(DateTime createdAt, string reason, string guildId, string moderatorId, string targetUserId)
        : base(createdAt, reason, guildId, moderatorId, targetUserId)
    {
    }
}
