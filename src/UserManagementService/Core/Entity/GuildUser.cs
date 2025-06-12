using GrillBot.Core.Database.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace UserManagementService.Core.Entity;

[PrimaryKey(nameof(GuildId), nameof(UserId))]
public class GuildUser
{
    public DiscordIdValueObject GuildId { get; set; }
    public DiscordIdValueObject UserId { get; set; }

    [StringLength(32)]
    public string? CurrentNickname { get; set; }
}
