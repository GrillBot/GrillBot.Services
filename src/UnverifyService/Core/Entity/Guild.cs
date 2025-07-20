using GrillBot.Core.Database.ValueObjects;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UnverifyService.Core.Entity;

public class Guild
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public DiscordIdValueObject GuildId { get; set; }

    public DiscordIdValueObject OwnerUserId { get; set; }
    public DiscordIdValueObject? MuteRoleId { get; set; }
}
