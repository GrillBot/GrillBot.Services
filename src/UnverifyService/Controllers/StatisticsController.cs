using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using UnverifyService.Actions.Statistics.PeriodStatistics;
using UnverifyService.Core.Enums;

namespace UnverifyService.Controllers;

public class StatisticsController(IServiceProvider serviceProvider) : GrillBot.Core.Infrastructure.Actions.ControllerBase(serviceProvider)
{
    [HttpGet("periodStats")]
    [ProducesResponseType<Dictionary<string, long>>(StatusCodes.Status200OK)]
    public Task<IActionResult> GetPeriodStatisticsAsync(
        [Required] string groupingKey,
        [Required] UnverifyOperationType operationType
    ) => ProcessAsync<GetUnverifyPeriodStatisticsAction>(groupingKey, operationType);
}
