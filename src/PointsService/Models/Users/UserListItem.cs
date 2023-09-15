namespace PointsService.Models.Users;

public class UserListItem
{
    public string GuildId { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public long ActivePoints { get; set; }
    public long ExpiredPoints { get; set; }
    public long MergedPoints { get; set; }
    public bool PointsDeactivated { get; set; }
}
