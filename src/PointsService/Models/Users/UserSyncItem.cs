namespace PointsService.Models.Users;

public class UserSyncItem
{
    public string Id { get; set; } = null!;
    public bool? PointsDisabled { get; set; }
    public bool? IsUser { get; set; }

    public UserSyncItem()
    {
    }

    public UserSyncItem(string id, bool? pointsDeactivated, bool? isUser)
    {
        Id = id;
        PointsDisabled = pointsDeactivated;
        IsUser = isUser;
    }
}
