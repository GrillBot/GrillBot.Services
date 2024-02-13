namespace PointsService.Models.Events;

public class UserRecalculationPayload
{
    public const string QueueName = "points:user_recalculation";

    public string GuildId { get; set; } = null!;
    public string UserId { get; set; } = null!;
}
