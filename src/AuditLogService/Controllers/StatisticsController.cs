using AuditLogService.Actions.Statistics;
using AuditLogService.Actions.Statistics.PeriodStatistics;
using AuditLogService.Models.Response.Statistics;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace AuditLogService.Controllers;

public class StatisticsController(IServiceProvider serviceProvider) : ControllerBase(serviceProvider)
{
    [HttpGet("auditLog")]
    [ProducesResponseType(typeof(AuditLogStatistics), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAuditLogStatisticsAsync()
        => await ProcessAsync<GetAuditLogStatisticsAction>();

    [HttpGet("interactions/stats")]
    [ProducesResponseType(typeof(InteractionStatistics), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInteractionStatisticsListAsync()
        => await ProcessAsync<GetInteractionStatisticsAction>();

    [HttpGet("interactions/userStats")]
    [ProducesResponseType(typeof(List<UserActionCountItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserCommandStatisticsAsync()
        => await ProcessAsync<GetUserCommandStatisticsAction>();

    [HttpGet("api/stats")]
    [ProducesResponseType(typeof(ApiStatistics), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApiStatisticsAsync()
        => await ProcessAsync<GetApiStatisticsAction>();

    [HttpGet("api/userStats/{criteria}")]
    [ProducesResponseType(typeof(List<UserActionCountItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUseApiStatisticsAsync(string criteria)
        => await ProcessAsync<GetUserApiStatisticsAction>(criteria);

    [HttpGet("avgTimes")]
    [ProducesResponseType(typeof(AvgExecutionTimes), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvgTimesAsync()
        => await ProcessAsync<GetAvgTimesAction>();

    [HttpGet("api/periodStats")]
    [ProducesResponseType<Dictionary<string, long>>(StatusCodes.Status200OK)]
    public Task<IActionResult> GetApiPeriodStatisticsAsync([FromQuery(Name = "group")] string[] apiGroups, [Required] string groupingKey)
        => ProcessAsync<GetApiPeriodStatisticsAction>(groupingKey, apiGroups);

    [HttpGet("auditLog/periodStats")]
    [ProducesResponseType<Dictionary<string, long>>(StatusCodes.Status200OK)]
    public Task<IActionResult> GetAuditLogPeriodStatisticsasync([Required] string groupingKey)
        => ProcessAsync<GetAuditLogPeriodStatisticsAction>(groupingKey);

    [HttpGet("interactions/periodStats")]
    [ProducesResponseType<Dictionary<string, long>>(StatusCodes.Status200OK)]
    public Task<IActionResult> GetInteractionsPeriodStatisticsAsync([Required] string groupingKey)
        => ProcessAsync<GetInteractionsPeriodStatisticsAction>(groupingKey);
}
