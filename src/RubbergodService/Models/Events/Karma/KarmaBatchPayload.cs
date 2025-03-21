namespace RubbergodService.Models.Events.Karma;

public class KarmaBatchPayload
{
    public List<KarmaUser> Users { get; set; } = [];

    public KarmaBatchPayload()
    {
    }

    public KarmaBatchPayload(IEnumerable<KarmaUser> users)
    {
        Users = users.ToList();
    }
}
