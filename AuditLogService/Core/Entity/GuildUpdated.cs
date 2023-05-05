namespace AuditLogService.Core.Entity;

public class GuildUpdated : ChildEntityBase
{
    public Guid BeforeId { get; set; }
    public Guid AfterId { get; set; }

    public GuildInfo Before { get; set; } = null!;
    public GuildInfo After { get; set; } = null!;
}
