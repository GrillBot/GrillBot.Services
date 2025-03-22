using GrillBot.Core.RabbitMQ.V2.Messages;

namespace RubbergodService.Models.Events.Karma;

public class KarmaBatchPayload : IRabbitMessage
{
    public string Topic => "Rubbergod";
    public string Queue => "KarmaBatch";

    public List<KarmaUser> Users { get; set; } = [];

    public KarmaBatchPayload()
    {
    }

    public KarmaBatchPayload(IEnumerable<KarmaUser> users)
    {
        Users = users.ToList();
    }
}
