using GrillBot.Core.RabbitMQ;
using SearchingService.Models.Events.Users;

namespace SearchingService.Models.Events;

public class SynchronizationPayload : IPayload
{
    public string QueueName => "searching:synchronization";

    public List<UserSynchronizationItem> Users { get; set; } = new();

    public SynchronizationPayload()
    {
    }

    public SynchronizationPayload(IEnumerable<UserSynchronizationItem> users)
    {
        Users = users.ToList();
    }
}
