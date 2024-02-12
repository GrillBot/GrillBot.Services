namespace UserMeasuresService.Models.Events;

public class UnverifyEvent : BaseEvent
{
    public const string QueueName = "user_measures:unverify";

    public DateTime EndAt { get; set; }
}
