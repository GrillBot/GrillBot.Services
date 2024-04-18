namespace PointsService.Models.Channels;

public class ChannelSyncItem
{
    public string Id { get; set; } = null!;
    public bool? IsDeleted { get; set; }
    public bool? PointsDisabled { get; set; }

    public ChannelSyncItem()
    {
    }

    public ChannelSyncItem(string id, bool? isDeleted, bool? pointsDeactivated)
    {
        Id = id;
        IsDeleted = isDeleted;
        PointsDisabled = pointsDeactivated;
    }
}
