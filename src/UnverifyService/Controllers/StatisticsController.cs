﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;
using UnverifyService.Actions.Statistics.PeriodStatistics;

namespace UnverifyService.Controllers;

public class StatisticsController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpGet("periodStats")]
    [ProducesResponseType<Dictionary<string, long>>(StatusCodes.Status200OK)]
    public Task<IActionResult> GetPeriodStatisticsAsync(
        [Required] string groupingKey,
        [Required] OperationType operationType
    ) => ProcessAsync<GetUnverifyPeriodStatisticsAction>(groupingKey, operationType);
}
