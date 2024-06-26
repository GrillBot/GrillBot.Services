using GrillBot.Core.Validation;
using System.ComponentModel.DataAnnotations;

namespace PointsService.Models;

public class IncrementPointsRequest
{
    [StringLength(30)]
    [DiscordId]
    public string GuildId { get; set; } = null!;

    [StringLength(30)]
    [DiscordId]
    public string UserId { get; set; } = null!;

    public int Amount { get; set; }
}
