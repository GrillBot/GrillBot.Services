﻿using Microsoft.AspNetCore.Mvc;
using PointsService.Actions;
using PointsService.Models;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace PointsService.Controllers;

public class LeaderboardController : ControllerBase
{
    public LeaderboardController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet("{guildId}")]
    [ProducesResponseType(typeof(Leaderboard), StatusCodes.Status200OK)]
    public Task<IActionResult> GetLeaderboardAsync([FromRoute]LeaderboardRequest request)
        => ProcessAsync<LeaderboardAction>(request);
}
