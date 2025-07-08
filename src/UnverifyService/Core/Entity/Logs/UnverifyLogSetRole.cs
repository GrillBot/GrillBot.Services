using GrillBot.Core.Database.ValueObjects;

namespace UnverifyService.Core.Entity.Logs;

public class UnverifyLogSetRole
{
    public Guid LogItemId { get; set; }
    public UnverifyLogSetOperation Operation { get; set; } = default!;

    public DiscordIdValueObject RoleId { get; set; }
    public bool IsKept { get; set; }
}
