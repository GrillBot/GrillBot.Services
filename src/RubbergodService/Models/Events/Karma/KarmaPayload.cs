using GrillBot.Core.RabbitMQ;

namespace RubbergodService.Models.Events.Karma;

public class KarmaPayload : IPayload
{
    public string QueueName => "rubbergod:store_karma";

    public string MemberId { get; set; } = null!;
    public int Karma { get; set; }
    public int Positive { get; set; }
    public int Negative { get; set; }

    public KarmaPayload()
    {
    }

    public KarmaPayload(string memberId, int karma, int positive, int negative)
    {
        MemberId = memberId;
        Karma = karma;
        Positive = positive;
        Negative = negative;
    }
}
