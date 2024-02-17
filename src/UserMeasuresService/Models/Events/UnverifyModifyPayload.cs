namespace UserMeasuresService.Models.Events;

public class UnverifyModifyPayload
{
    public const string QueueName = "user_measures:unverify_modify";

    public long LogSetId { get; set; }
    public DateTime? NewEnd { get; set; }

    public UnverifyModifyPayload()
    {
    }

    public UnverifyModifyPayload(long logSetId, DateTime? newEnd)
    {
        LogSetId = logSetId;
        NewEnd = newEnd;
    }
}
