namespace PointsService.Models.Users;

public class UserSyncItem
{
    public string Id { get; set; } = null!;
    public bool? PointsDisabled { get; set; }
    public bool? IsUser { get; set; }
}
