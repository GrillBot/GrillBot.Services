using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;

namespace PointsService.Models;

public class TransactionRequest
{
    [Required]
    [StringLength(30)]
    [DiscordId]
    public string GuildId { get; set; } = null!;

    [Required]
    [StringLength(30)]
    [DiscordId]
    public string ChannelId { get; set; } = null!;

    [Required]
    public MessageInfo MessageInfo { get; set; } = null!;

    public ReactionInfo? ReactionInfo { get; set; }
}
