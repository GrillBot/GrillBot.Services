using SearchingService.Models.Events.Users;

namespace SearchingService.Models.Events;

public class SynchronizationPayload
{
    public List<UserSynchronizationItem> Users { get; set; } = [];

    public SynchronizationPayload()
    {
    }

    public SynchronizationPayload(IEnumerable<UserSynchronizationItem> users)
    {
        Users = users.ToList();
    }
}
