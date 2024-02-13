namespace PointsService.Models.Events;

public abstract class CreateTransactionBasePayload
{
    public string GuildId { get; set; } = null!;
}
