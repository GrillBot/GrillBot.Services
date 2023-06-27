using AuditLogService.Actions.Dashboard;
using AuditLogService.Models.Response.Info.Dashboard;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = GrillBot.Core.Infrastructure.Actions.ControllerBase;

namespace AuditLogService.Controllers;

public class DashboardController : ControllerBase
{
    public DashboardController(IServiceProvider serviceProvider) : base(serviceProvider)
    {
    }

    [HttpGet("api/{apiGroup}")]
    [ProducesResponseType(typeof(List<DashboardInfoRow>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApiDashboardAsync(string apiGroup)
        => await ProcessAsync<GetApiDashboardAction>(apiGroup);

    [HttpGet("interactions")]
    [ProducesResponseType(typeof(List<DashboardInfoRow>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInteractionsDashboardAsync()
        => await ProcessAsync<GetInteractionsDashboardAction>();
    
    [HttpGet("jobs")]
    [ProducesResponseType(typeof(List<DashboardInfoRow>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetJobsDashboardAsync()
        => await ProcessAsync<GetJobsDashboardListAction>();
    
    [HttpGet("todayAvgTimes")]
    [ProducesResponseType(typeof(List<DashboardInfoRow>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTodayAvgTimesAsync()
        => await ProcessAsync<GetTodayAvgTimesDashboard>();
}
