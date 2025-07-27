using GrillBot.Core.Validation;
using System.ComponentModel.DataAnnotations;

namespace UnverifyService.Models.Request;

public class UpdateUnverifyRequest
{
    [DiscordId]
    [StringLength(32)]
    public string GuildId { get; set; } = null!;

    [DiscordId]
    [StringLength(32)]
    public string UserId { get; set; } = null!;

    public DateTime NewEndAtUtc { get; set; }
    public string? Reason { get; set; }
}
