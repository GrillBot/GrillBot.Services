using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;
using Microsoft.AspNetCore.Mvc;
using PointsService.Actions;
using PointsService.Models;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace PointsService.Controllers;

public class StatusController : ControllerBase
{
    public StatusController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet("{guildId}/{userId}")]
    [ProducesResponseType(typeof(PointsStatus), StatusCodes.Status200OK)]
    public Task<IActionResult> GetStatusOfPointsAsync([Required, StringLength(30), DiscordId] string guildId, [Required, StringLength(30), DiscordId] string userId)
        => ProcessAsync<CurrentPointsStatusAction>(guildId, userId, false);

    [HttpGet("{guildId}/{userId}/expired")]
    [ProducesResponseType(typeof(PointsStatus), StatusCodes.Status200OK)]
    public Task<IActionResult> GetStatusOfExpiredPointsAsync([Required, StringLength(30), DiscordId] string guildId, [Required, StringLength(30), DiscordId] string userId)
        => ProcessAsync<CurrentPointsStatusAction>(guildId, userId, true);

    [HttpGet("{guildId}/{userId}/image")]
    public Task<IActionResult> GetImagePointsStatusAsync([Required, StringLength(30), DiscordId] string guildId, [Required, StringLength(30), DiscordId] string userId)
        => ProcessAsync<ImagePointsStatusAction>(guildId, userId);
}
