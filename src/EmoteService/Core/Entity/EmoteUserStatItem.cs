using EmoteService.Core.Entity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EmoteService.Core.Entity;

[PrimaryKey(nameof(EmoteId), nameof(EmoteName), nameof(EmoteIsAnimated), nameof(GuildId), nameof(UserId))]
public class EmoteUserStatItem : EmoteValueObject
{
    [StringLength(32)]
    public string GuildId { get; set; } = null!;

    [StringLength(32)]
    public string UserId { get; set; } = null!;

    public long UseCount { get; set; }

    public DateTime FirstOccurence { get; set; }
    public DateTime LastOccurence { get; set; }
}
