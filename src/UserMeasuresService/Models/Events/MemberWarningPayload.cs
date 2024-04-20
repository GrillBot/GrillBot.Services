namespace UserMeasuresService.Models.Events;

public class MemberWarningPayload : BasePayload
{
    public override string QueueName => "user_measures:member_warning";

    public bool SendDmNotification { get; set; }

    public MemberWarningPayload()
    {
    }

    public MemberWarningPayload(DateTime createdAtUtc, string reason, string guildId, string moderatorId, string targetUserId, bool sendDmNotification)
        : base(createdAtUtc, reason, guildId, moderatorId, targetUserId)
    {
        SendDmNotification = sendDmNotification;
    }
}
