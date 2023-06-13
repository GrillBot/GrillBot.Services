using AuditLogService.Actions.Statistics;
using AuditLogService.Models.Response.Statistics;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace AuditLogService.Controllers;

public class StatisticsController : ControllerBase
{
    public StatisticsController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet("auditLog")]
    [ProducesResponseType(typeof(AuditLogStatistics), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAuditLogStatisticsAsync()
        => await ProcessAsync<GetAuditLogStatisticsAction>();

    [HttpGet("interactions/list")]
    [ProducesResponseType(typeof(List<StatisticItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInteractionStatisticsListAsync()
        => await ProcessAsync<GetInteractionStatisticsListAction>();

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
}
