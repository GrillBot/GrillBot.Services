using GrillBot.Core.Database.ValueObjects;

namespace UnverifyService.Core.Entity.Logs;

public class UnverifyLogRemoveRole
{
    public Guid LogItemId { get; set; }
    public UnverifyLogRemoveOperation Operation { get; set; } = default!;

    public DiscordIdValueObject RoleId { get; set; }
}
