using GrillBot.Core.Database.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UnverifyService.Core.Enums;

namespace UnverifyService.Core.Entity.Logs;

public class UnverifyLogItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.NewGuid();

    public UnverifyOperationType OperationType { get; set; }
    public DiscordIdValueObject GuildId { get; set; }
    public DiscordIdValueObject FromUserId { get; set; }
    public DiscordIdValueObject ToUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid? ParentLogItemId { get; set; }
    public UnverifyLogItem? ParentLogItem { get; set; }
    public IList<UnverifyLogItem> ChildLogItems { get; set; } = [];

    public UnverifyLogSetOperation? SetOperation { get; set; }
    public UnverifyLogRemoveOperation? RemoveOperation { get; set; }
    public UnverifyLogUpdateOperation? UpdateOperation { get; set; }
}
