using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SearchingService.Core.Entity;

[PrimaryKey(nameof(GuildId), nameof(UserId))]
public class User
{
    [StringLength(30)]
    public string GuildId { get; set; } = null!;

    [StringLength(30)]
    public string UserId { get; set; } = null!;

    public bool IsSearchingAdmin { get; set; }
    public bool HaveGuildAdmin { get; set; }
    public bool HaveManageMessages { get; set; }

    [NotMapped]
    public bool IsAdministrator
        => IsSearchingAdmin || HaveGuildAdmin || HaveManageMessages;
}
