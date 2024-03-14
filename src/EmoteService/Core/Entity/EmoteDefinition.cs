using EmoteService.Core.Entity.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EmoteService.Core.Entity;

[PrimaryKey(nameof(EmoteId), nameof(EmoteName), nameof(EmoteIsAnimated), nameof(GuildId))]
public class EmoteDefinition : EmoteValueObject
{
    [StringLength(32)]
    public string GuildId { get; set; } = null!;
}
