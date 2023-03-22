using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;

namespace PointsService.Models;

public class SynchronizationRequest
{
    [Required]
    [StringLength(30)]
    [DiscordId]
    public string GuildId { get; set; } = null!;

    public List<ChannelInfo> Channels { get; set; } = new();

    public List<UserInfo> Users { get; set; } = new();
}
