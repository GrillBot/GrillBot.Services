using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InviteService.Core.Entity;

[PrimaryKey(nameof(Code), nameof(GuildId))]
public class Invite
{
    [StringLength(10)]
    public string Code { get; set; } = null!;

    [StringLength(32)]
    public string GuildId { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    [StringLength(32)]
    public string? CreatorId { get; set; }

    public IList<InviteUse> Uses { get; set; } = [];
}
