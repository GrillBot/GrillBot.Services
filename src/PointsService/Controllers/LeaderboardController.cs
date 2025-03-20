﻿using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;
using Microsoft.AspNetCore.Mvc;
using PointsService.Actions;
using PointsService.Models;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace PointsService.Controllers;

public class LeaderboardController(IServiceProvider serviceProvider) : ControllerBase(serviceProvider)
{
    [HttpGet("{guildId}")]
    [ProducesResponseType(typeof(List<BoardItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetLeaderboardAsync([FromRoute] LeaderboardRequest request)
        => ProcessAsync<GetLeaderboardAction>(request);

    [HttpGet("{guildId}/count")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public Task<IActionResult> GetLeaderboardCountAsync([Required, StringLength(30), DiscordId] string guildId)
        => ProcessAsync<GetLeaderboardCountAction>(guildId);
}
