using GrillBot.Core.Database.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagementService.Core.Entity;

public class GuildUserNickname
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; } = Guid.NewGuid();

    public DiscordIdValueObject GuildId { get; set; }
    public DiscordIdValueObject UserId { get; set; }

    [StringLength(32)]
    public string Value { get; set; } = null!;

    public DateTime InsertedAtUtc { get; set; } = DateTime.UtcNow;

    public GuildUser GuildUser { get; set; } = null!;
}
