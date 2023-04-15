using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;
using Microsoft.AspNetCore.Mvc;

namespace PointsService.Models;

public class LeaderboardRequest
{
    [Required]
    [DiscordId]
    [StringLength(30)]
    [FromRoute]
    public string GuildId { get; set; } = null!;

    [FromQuery]
    public int Skip { get; set; }

    [FromQuery]
    public int Count { get; set; }
    
    [FromQuery]
    public bool Simple { get; set; }
}
