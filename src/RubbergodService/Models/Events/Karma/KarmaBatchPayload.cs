using GrillBot.Core.RabbitMQ;

namespace RubbergodService.Models.Events.Karma;

public class KarmaBatchPayload : IPayload
{
    public string QueueName => "rubbergod:store_karma";

    public List<KarmaUser> Users { get; set; } = new();

    public KarmaBatchPayload()
    {
    }

    public KarmaBatchPayload(IEnumerable<KarmaUser> users)
    {
        Users = users.ToList();
    }
}
