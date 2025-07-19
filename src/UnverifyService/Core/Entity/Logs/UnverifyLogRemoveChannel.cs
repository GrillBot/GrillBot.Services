using Discord;
using GrillBot.Core.Database.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace UnverifyService.Core.Entity.Logs;

public class UnverifyLogRemoveChannel
{
    public Guid LogItemId { get; set; }
    public UnverifyLogRemoveOperation Operation { get; set; } = default!;

    public DiscordIdValueObject ChannelId { get; set; }
    public ulong AllowValue { get; set; }
    public ulong DenyValue { get; set; }

    [NotMapped]
    public OverwritePermissions Perms => new(AllowValue, DenyValue);
}
