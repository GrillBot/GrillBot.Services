namespace UserMeasuresService.Models.Events;

public class MemberWarningPayload : BasePayload
{
    public const string QueueName = "user_measures:member_warning";
}
