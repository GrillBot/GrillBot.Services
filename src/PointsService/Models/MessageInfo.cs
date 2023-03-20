using System.ComponentModel.DataAnnotations;
using Discord;
using GrillBot.Core.Validation;

namespace PointsService.Models;

public class MessageInfo
{
    [Required]
    [StringLength(30)]
    [DiscordId]
    public string Id { get; set; } = null!;

    [Required]
    public int ContentLength { get; set; }
    
    [Required]
    public MessageType MessageType { get; set; }

    [Required]
    public UserInfo Author { get; set; } = null!;
}
