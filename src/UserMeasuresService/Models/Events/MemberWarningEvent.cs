namespace UserMeasuresService.Models.Events;

public class MemberWarningEvent : BaseEvent
{
    public const string QueueName = "user_measures:member_warning";
}
